using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Jaeger;
using OpenTracing;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using OpenTracing.Contrib.NetCore.Configuration;
using Jaeger.Senders;
using OpenTracing.Util;
using Unleash;

namespace api_under_test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //https://thecloudblog.net/post/distributed-tracing-in-asp.net-core-with-jaeger-and-tye-part-1-distributed-tracing/
            services.AddOpenTracing();

            services.AddSingleton<ITracer>(sp =>
            {
                var serviceName = sp.GetRequiredService<IWebHostEnvironment>().ApplicationName;
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                Jaeger.Configuration.SenderConfiguration.DefaultSenderResolver = new SenderResolver(loggerFactory)
                    .RegisterSenderFactory<ThriftSenderFactory>();

                var reporter = new RemoteReporter.Builder().WithLoggerFactory(loggerFactory)
                    .WithSender(new UdpSender("172.17.0.1", 6831, 65000))
                    .Build();

                var tracer = new Tracer.Builder(serviceName)
                    .WithLoggerFactory(loggerFactory)
                    // The constant sampler reports every span.
                    .WithSampler(new ConstSampler(true))
                    // LoggingReporter prints every reported span to the logging framework.
                    .WithReporter(reporter)
                    .Build();
                GlobalTracer.Register(tracer);
                return tracer;
            });
            services.Configure<HttpHandlerDiagnosticOptions>(options =>
                        options.OperationNameResolver =
                        request => $"{request.Method.Method}: {request?.RequestUri?.AbsoluteUri}");

            var unleashSecret = "ee25449d8edec0cf218cc68b484f4cfa72ca0f8db4b913fe62784bbdeb59a255";

            var unleashSettings = new UnleashSettings
            {
                AppName = "api-under-test",
                InstanceTag = Environment.MachineName,
                SendMetricsInterval = TimeSpan.FromMinutes(1),
                UnleashApi = new Uri("http://172.17.0.1:4242/api/"),
                CustomHttpHeaders = new Dictionary<string, string> {{"Authorization", unleashSecret}}
            };

            services.AddSingleton<IUnleash>(ctx => new DefaultUnleash(unleashSettings));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

//            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
