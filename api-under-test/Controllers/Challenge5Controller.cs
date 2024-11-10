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
    [Route("weatherforecast_challenge5")]
    public class Challenge5Controller: ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Challenge5Controller> _logger;
        private readonly Random _rng; 

        public Challenge5Controller(ILogger<Challenge5Controller> logger)
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
            return await policy.ExecuteAsync(() => GetForecasts());
        }

        private IAsyncPolicy GetPolicy() {
            var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromMilliseconds(20));
            return timeoutPolicy; 
        }

        // This signature needs to change to accept a CancellationToken 
        private async Task<IEnumerable<WeatherForecast>> GetForecasts()
        {
            // And this too must propagate tokens 
            var tasks = Enumerable.Range(1, 3).Select(index => GetForecast(index)).ToArray();
            await Task.WhenAll(tasks);
            return tasks.Select(t => t.Result).ToArray();
        }

        // The CancellationToken needs to go here as well 
        private async Task<WeatherForecast> GetForecast(int index){
            
            // The delays need to cooperate with the cancellation by being handled a token 
            // https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.delay?view=netcore-3.1

            // (Also other async methods in .NET accept CancellationTokens, for example:
            // https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.sendasync?view=netcore-3.1)
            if (_rng.NextDouble() > 0.95) {
               await Task.Delay(2000);
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