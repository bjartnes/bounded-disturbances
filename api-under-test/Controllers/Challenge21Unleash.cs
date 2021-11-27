using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Latency;
using Polly.Contrib.Simmy.Outcomes;
using System.Threading;
using OpenTracing;
using Unleash;
using System.Net.Http;
using IHttpClientFactory = System.Net.Http.IHttpClientFactory;

namespace api_under_test.Controllers
{
    [ApiController]
    [Route("weatherforecast_challenge21")]
    public class Challenge21Controller: ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Challenge21Controller> _logger;
        private readonly ITracer _tracer;
        private readonly IUnleash _unleash;
		private readonly IHttpClientFactory _clientFactory;

		public Challenge21Controller(ILogger<Challenge21Controller> logger,
                                    ITracer tracer,
                                    IUnleash unleash,
				                    IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _tracer = tracer;
            _unleash = unleash;
			_clientFactory = clientFactory;
		}

        [HttpGet]
        public async Task<string> Get()
        {
            var actionName = ControllerContext.ActionDescriptor.DisplayName;
            using (var scope = _tracer.BuildSpan(actionName).StartActive(true)) {
                var chaosPolicy = MonkeyPolicy.InjectLatencyAsync(with =>
                 with.Latency(TimeSpan.FromSeconds(30))
                    .InjectionRate(0.85)
                    .Enabled(false));
                var mix = Policy.WrapAsync(GetPolicy(), chaosPolicy);
                return await mix.ExecuteAsync((ct) => GetForecasts(ct), CancellationToken.None);
            }
        }

        private IAsyncPolicy GetPolicy() {
            // Fill inn answer by changing code from here
            var retries = 0;
            Program.ConfiguredRetries.Set(retries);
            Program.ConfiguredRetries.Publish();
            var policy = Policy.Handle<Exception>().RetryAsync(retries, (ex, attempt) => Program.ExecutedRetries.Inc());

           return policy; 
        }
        private async Task<string> GetForecasts(CancellationToken ct)
        {
            // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#httpclient-lifetimes
            var useHttpClientFactory = _unleash.IsEnabled("useHttpClientFactory");
            if (useHttpClientFactory) {
                var client = _clientFactory.CreateClient(); 
                return await client.GetStringAsync("http://172.17.0.1:5000/weatherforecast_intro");
            } else {
                using var client = new HttpClient(); 
                return await client.GetStringAsync("http://172.17.0.1:5000/weatherforecast_intro");
            }
        }
    }
}