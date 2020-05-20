using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Latency;
using System.Threading;

namespace api_under_test.Controllers
{
    [ApiController]
    [Route("weatherforecast_challenge2")]
    public class Challenge2Controller: ControllerBase
    { private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Challenge2Controller> _logger;

        public Challenge2Controller(ILogger<Challenge2Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
           var chaosPolicy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(TimeSpan.FromSeconds(1))
                    .InjectionRate(0.05) // 10 % 
                    .Enabled(true));    // Would probably only turn it on in some environments
            var mix = Policy.WrapAsync(GetPolicy(), chaosPolicy);
            return await mix.ExecuteAsync((ct) => GetForecasts(ct), CancellationToken.None);
        }

        private IAsyncPolicy GetPolicy() {
//          Fill inn answer by changing code from here
//          var retryPolicy = Policy.Handle<Exception>().RetryAsync(4);
//          var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromMilliseconds(400));
//          var policy = Policy.WrapAsync(retryPolicy, timeoutPolicy);
           var policy = Policy.Handle<Exception>().RetryAsync(0);
//          until here
            return policy; 
        }

        private async Task<IEnumerable<WeatherForecast>> GetForecasts(CancellationToken ct)
        {
            await Task.Delay(1, ct);
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
