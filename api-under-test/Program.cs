using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;
using Prometheus.DotNetRuntime;

namespace api_under_test
{
    public class Program
    {

        public static Gauge Retries = Metrics.CreateGauge("polly_retries", "How many");

        public static void Main(string[] args)
        {
            var collector = DotNetRuntimeStatsBuilder.Default().StartCollecting();
            //var server = new MetricServer(hostname: "*", port: 1234);
            var server = new MetricServer(hostname: "*", port: 1234);
            
            server.Start();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://*:5000", "https://*:5001");
                });
    }
}
