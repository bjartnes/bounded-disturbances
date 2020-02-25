using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using System.Threading;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Latency;

namespace api_under_test.Controllers
{
    [ApiController]
    [Route("weatherforecast_challenge2")]
    public class Challenge2Controller: ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Challenge1Controller> _logger;
        private readonly Random _rng; 

        public Challenge2Controller(ILogger<Challenge1Controller> logger)
        {
            _rng = new Random();
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var policy = GetPolicy();
            return await policy.ExecuteAsync(GetForecasts); 
        }

        private IAsyncPolicy GetPolicy() {
            var policy = Policy.Handle<Exception>().RetryAsync(5);
            return policy; 
        }

        private async Task<IEnumerable<WeatherForecast>> GetForecasts()
        {
            var tasks = Enumerable.Range(1, 3).Select(index => GetForecastService(index)).ToArray();
            await Task.WhenAll(tasks);
            return tasks.Select(t => t.Result).ToArray();
        }

        private Task<WeatherForecast> GetForecastService(int index){
            
            var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromMilliseconds(100));
            // There is a good hint in the example https://github.com/App-vNext/Polly/wiki/Timeout that mentions CancellationToken.None
            return timeoutPolicy.ExecuteAsync(() => GetForecast(index));
        }

        // There might be some changes required to this signature as well
        private async Task<WeatherForecast> GetForecast(int index){
            
            // The delay might need to cooperate with the cancellation 
            if (_rng.NextDouble() > 0.95) {
               await Task.Delay((int)(2000));
            } else {
                await Task.Delay(10);
            }

            return new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = _rng.Next(-20, 55),
                Summary = Summaries[_rng.Next(Summaries.Length)]
            };
       }
    }
}
