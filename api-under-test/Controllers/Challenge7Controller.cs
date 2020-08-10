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
    [Route("weatherforecast_challenge7/{id}")]
    public class Challenge7Controller: ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Challenge7Controller> _logger;

        public Challenge7Controller(ILogger<Challenge7Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get(int id)
        {
            var latency = TimeSpan.FromMilliseconds(2000);
            var injectionRate = 0.1; 
            var latencyMonkey = MonkeyPolicy.InjectLatencyAsync(with =>
                        with.Latency(latency)
                            .InjectionRate(injectionRate) 
                            .Enabled(true));

            var mix = Policy.WrapAsync(GetPolicy(), latencyMonkey);

            return await mix.ExecuteAsync((ct) => GetForecasts(id, ct), CancellationToken.None);
        }

        private IAsyncPolicy GetPolicy() {
            // https://stackoverflow.com/questions/49344841/adjusting-timeout-duration-based-on-retry-count

            const string RetryCountKey = "RetryCount";

            // It should be enough just adding the rights numbers in the timeouts array, maybe you need more of them too:
            var timeouts = new [] { 666, 666 };

            var retries = timeouts.Count()-1;
            var retryStoringRetryCount = Policy
                .Handle<Exception>()
                .RetryAsync(retries, (exception, retryCount, context) =>
                {
                    Console.WriteLine("Storing retry count of " + retryCount + " in execution context.");
                    context[RetryCountKey] = retryCount;
                });

            var timeoutBasedOnRetryCount = Policy
                .TimeoutAsync(context =>
                {
                    int tryCount;
                    try
                    {
                        tryCount = (int) context[RetryCountKey];
                    }
                    catch
                    {
                        tryCount = 0; // choose your own default for when it is not set; also applies to first try, before any retries
                    }
                    if (tryCount > 0) {
                        Program.ExecutedRetries.Inc();
                    }
                    int timeoutMs = timeouts.Skip(tryCount).First(); 
                    Console.WriteLine("Obtained retry count of " + tryCount + " from context, thus timeout is " + timeoutMs + " ms.");
                    return TimeSpan.FromMilliseconds(timeoutMs);
                });


            return Policy.WrapAsync(retryStoringRetryCount, timeoutBasedOnRetryCount);
        }

        private async Task<IEnumerable<WeatherForecast>> GetForecasts(int id, CancellationToken ct)
        {
            var timeToFetchResource = id ==1 ? 1000 : 20; 
            await Task.Delay(timeToFetchResource, ct);
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