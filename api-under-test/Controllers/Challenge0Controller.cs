using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;

namespace api_under_test.Controllers
{
    [ApiController]
    [Route("weatherforecast_challenge0")]
    public class Challenge0Controller: ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Challenge0Controller> _logger;

        public Challenge0Controller(ILogger<Challenge0Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            Exception fault = new System.Net.Sockets.SocketException(errorCode: 10013);

            var chaosPolicy = MonkeyPolicy.InjectExceptionAsync(with 
            => with.Fault(fault)
                   .InjectionRate(0.1)
                   .Enabled(true));
            var mix = Policy.WrapAsync(GetPolicy(), chaosPolicy);
            return await mix.ExecuteAsync(GetForecasts);
        }

        private IAsyncPolicy GetPolicy() {
            // FIll inn answer here
            var policy = Policy.Handle<Exception>().RetryAsync(0);
            return policy; 
        }
        // Should probably get them from yr , but will of course cause a lot of load
        // from a workshop.. Perhaps better to simulate
        private async Task<IEnumerable<WeatherForecast>> GetForecasts()
        {
            await Task.Delay(20);
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