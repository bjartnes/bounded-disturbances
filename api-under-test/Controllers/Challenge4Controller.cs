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

namespace api_under_test.Controllers
{
    [ApiController]
    [Route("weatherforecast_challenge4")]
    public class Challenge4Controller: ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Challenge4Controller> _logger;

        public Challenge4Controller(ILogger<Challenge4Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
           Exception fault = new System.Net.Sockets.SocketException(errorCode: 10013);
           var latencyMonkey = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(TimeSpan.FromSeconds(1))
                    .InjectionRate(0.1) 
                    .Enabled(true));

            var errorMonkey = MonkeyPolicy.InjectExceptionAsync(with => 
                with.Fault(fault)
                    .InjectionRate(0.1)
                    .Enabled(true));
 
            var monkeyPolicy = Policy.WrapAsync(latencyMonkey, errorMonkey);

            var mix = Policy.WrapAsync(GetPolicy(), monkeyPolicy);
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
            return Policy.WrapAsync(retryPolicy, timeoutPolicy);
            // until here
        }

        private async Task<IEnumerable<WeatherForecast>> GetForecasts(CancellationToken ct)
        {
            await Task.Delay(20, ct);
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