using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using System.Threading;
using System.Net.Http;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Latency;

namespace api_under_test.Controllers
{
    [ApiController]
    [Route("weatherforecast_challenge10")]
    public class Challenge10Controller: ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Challenge10Controller> _logger;
        private readonly Random _rng; 

        public Challenge10Controller(ILogger<Challenge10Controller> logger)
        {
            _rng = new Random();
            _logger = logger;
        }

        [HttpGet]
        // We should be able to save our API from running out of sockets by simply changing the Get() method - both the signature and the CancellationToken.None part
        // You can have a CancellationToken by just adding it to the controller method - frameworks aren't always evil
//        public async Task<string> Get(CancellationToken ct)
        public async Task<string> Get()
        {
            var policy = GetPolicy();
            // We can also use someone elses token, the rest should be quite similar to challenge 4
            return await policy.ExecuteAsync((ct) => GetForecasts(ct), CancellationToken.None); 
        }

        private IAsyncPolicy GetPolicy() {
            return Policy.TimeoutAsync(TimeSpan.FromMilliseconds(2000));
        }

        private async Task<string> GetForecasts(CancellationToken ct)
        {
            var url = new Uri(@"https://localhost:5001/weatherforecast_challenge0");
            using (var client = new HttpClient()) {
                var resp = client.GetAsync(url, ct);
                await Task.Delay(1000, ct);
                return await (await resp).Content.ReadAsStringAsync();; 
            }
        }
    }
}