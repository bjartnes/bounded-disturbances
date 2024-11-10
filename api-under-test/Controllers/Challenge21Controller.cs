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
		private readonly IHttpClientFactory _clientFactory;

		public Challenge21Controller(ILogger<Challenge21Controller> logger,
				                    IHttpClientFactory clientFactory)
        {
            _logger = logger;
			_clientFactory = clientFactory;
		}

        [HttpGet]
        public async Task<string> Get()
        {
            return await GetForecasts(CancellationToken.None);
        }
       private async Task<string> GetForecasts(CancellationToken ct)
        {
            // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#httpclient-lifetimes
            var useHttpClientFactory = true; 
            if (useHttpClientFactory) {
                var client = _clientFactory.CreateClient(); 
                return await client.GetStringAsync("http://172.17.0.1:5555/weatherforecast_intro");
            } else {
                using var client = new HttpClient(); 
                return await client.GetStringAsync("http://172.17.0.1:5555/weatherforecast_intro");
            }
        }
    }
}