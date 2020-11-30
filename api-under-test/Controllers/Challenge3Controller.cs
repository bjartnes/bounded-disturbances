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
    [Route("weatherforecast_challenge3")]
    public class Challenge3Controller: ControllerBase
    { private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Challenge3Controller> _logger;

        public Challenge3Controller(ILogger<Challenge3Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
           var chaosPolicy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(TimeSpan.FromSeconds(1))
                    .InjectionRate(0.10) // 10 % 
                    .Enabled(true));    // Would probably only turn it on in some environments
            var mix = Policy.WrapAsync(GetPolicy(), chaosPolicy);
            return await mix.ExecuteAsync((ct) => GetForecasts(ct), CancellationToken.None);
        }

        private IAsyncPolicy GetPolicy() {
            // you can change from here 
            var retries = 0;
            var timeout = TimeSpan.FromMilliseconds(3000);
            Program.ConfiguredTimeout.Set(timeout.TotalMilliseconds);
            Program.ConfiguredRetries.Set(retries);

            Program.ConfiguredRetries.Publish();
            Program.ConfiguredTimeout.Publish();

            var retryPolicy = Policy.Handle<Exception>().RetryAsync(retries, (ex, attempt) => Program.ExecutedRetries.Inc());
            var timeoutPolicy = Policy.TimeoutAsync(timeout);
            return Policy.WrapAsync(timeoutPolicy, retryPolicy);
            // until here
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
