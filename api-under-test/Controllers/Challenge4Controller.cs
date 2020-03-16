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
    [Route("weatherforecast_challenge4")]
    public class Challenge4Controller: ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Challenge4Controller> _logger;
        private readonly Random _rng; 

        public Challenge4Controller(ILogger<Challenge4Controller> logger)
        {
            _rng = new Random();
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var policy = GetPolicy();
            // Unfortunately, in the real world there is a little more to cancellation.
            // It is explained here: 
            // https://github.com/App-vNext/Polly/wiki/Timeout
            // The example with CancellationToken.None shows a way to implement it
            // if you want to provide a new cancellation token that executeasync can use to terminate a task
            // The thing is, tasks must collaborate on cancellation
            // and they do that by sending cancellationtokens... They must come from somewhere 
            return await policy.ExecuteAsync((ct) => GetForecasts(ct), CancellationToken.None);
        }

        private IAsyncPolicy GetPolicy() {
            var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromMilliseconds(20));
            return timeoutPolicy; 
        }

        // This signature needs to change to accept a token 
        private async Task<IEnumerable<WeatherForecast>> GetForecasts(CancellationToken ct)
        {
            // And this too must propagate tokens 
            var tasks = Enumerable.Range(1, 3).Select(index => GetForecast(index, ct)).ToArray();
            await Task.WhenAll(tasks);
            return tasks.Select(t => t.Result).ToArray();
        }

        // There might be some changes required to this signature as well
        private async Task<WeatherForecast> GetForecast(int index, CancellationToken ct){
            
            // The delays need to cooperate with the cancellation by being handled a token 

            if (_rng.NextDouble() > 0.95) {
               await Task.Delay(2000, ct);
            } else {
                await Task.Delay(10, ct);
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