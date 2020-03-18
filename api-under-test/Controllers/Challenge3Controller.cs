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
using Polly.Contrib.Simmy.Behavior;
using System.Threading;

namespace api_under_test.Controllers
{
    [ApiController]
    [Route("weatherforecast_challenge3")]
    public class Challenge3Controller: ControllerBase
    {
        private static readonly string[] Summaries = new[]
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
            var mix = Policy.WrapAsync<IEnumerable<WeatherForecast>>(GetPolicy(), GetMonkeyPolicy());
            return await mix.ExecuteAsync((ct) => GetForecasts(ct), CancellationToken.None);
        }

        private IAsyncPolicy<IEnumerable<WeatherForecast>> GetMonkeyPolicy() {
           Exception fault = new System.Net.Sockets.SocketException(errorCode: 10013);

           var latencyMonkey = MonkeyPolicy.InjectLatencyAsync<IEnumerable<WeatherForecast>>(with =>
                with.Latency(TimeSpan.FromSeconds(1))
                    .InjectionRate(0.1) 
                    .Enabled(true));

           var latencyMonkey2 = MonkeyPolicy.InjectLatencyAsync<IEnumerable<WeatherForecast>>(with =>
                with.Latency(TimeSpan.FromSeconds(1))
                    .InjectionRate(0.1) 
                    .Enabled(true));

            var errorMonkey = MonkeyPolicy.InjectBehaviourAsync<IEnumerable<WeatherForecast>>(with => 
                with.Behaviour(() => throw fault) 
                    .InjectionRate(0.1)
                    .Enabled(true));
 
            return Policy.WrapAsync<IEnumerable<WeatherForecast>>(latencyMonkey, errorMonkey);
        } 
        private IAsyncPolicy<IEnumerable<WeatherForecast>> GetPolicy() {
//          Fill inn answer by changing code from here
            var retryPolicy = Policy<IEnumerable<WeatherForecast>>.Handle<Exception>().RetryAsync(3);
            var timeoutPolicy = Policy.TimeoutAsync<IEnumerable<WeatherForecast>>(TimeSpan.FromMilliseconds(50));
            var policy = Policy.WrapAsync<IEnumerable<WeatherForecast>>(retryPolicy, timeoutPolicy);
//          until here
            return policy; 
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