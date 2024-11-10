using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using System.Threading;
using System.IO;
using System.Net.Http;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Latency;
using System.Text;

namespace api_under_test.Controllers
{
    [ApiController]
    [Route("weatherforecast_challenge11")]
    public class Challenge11Controller: ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Challenge11Controller> _logger;
        private readonly Random _rng; 

        public Challenge11Controller(ILogger<Challenge11Controller> logger)
        {
            _rng = new Random();
            _logger = logger;
        }

        [HttpGet]
        //public async Task<string> Get(CancellationToken ct)
        public async Task<string> Get()
        {
            var policy = GetPolicy();
            // We can also use someone elses token, the rest should be quite similar to challenge 5
            return await policy.ExecuteAsync((ct) => GetForecasts(ct), CancellationToken.None); 
        }

        private IAsyncPolicy GetPolicy() {
            return Policy.TimeoutAsync(TimeSpan.FromMilliseconds(5000));
        }

        private async Task<int> Recursive(int counter, CancellationToken ct) {
            await Task.Delay(1);
            if (counter == 100) {
                System.Environment.Exit(1); //BOOM
            }
             if (ct.IsCancellationRequested) {
                return counter;
            }
            return await Recursive(counter+ 1, ct);
        }
        private async Task<string> GetForecasts(CancellationToken ct)
        {
            if (_rng.NextDouble() > 0.95) {
                return (await Recursive(1, ct)).ToString(); 
            } else {
                return "lucky number";
            }
        }
    }
}