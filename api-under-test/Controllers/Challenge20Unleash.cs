using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;
using System.Threading;
using OpenTracing;
using Unleash;

namespace api_under_test.Controllers
{
    [ApiController]
    [Route("weatherforecast_challenge_unleash")]
    public class Challenge20Controller: ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Challenge1Controller> _logger;
        private readonly ITracer _tracer;
        private readonly IUnleash _unleash;

        public Challenge20Controller(ILogger<Challenge1Controller> logger,
                                    ITracer tracer,
                                    IUnleash unleash)
        {
            _logger = logger;
            _tracer = tracer;
            _unleash = unleash;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var fooToggle = _unleash.IsEnabled("foo");
            if (fooToggle) throw new NotFiniteNumberException();
            var actionName = ControllerContext.ActionDescriptor.DisplayName;
            Console.WriteLine(_tracer);
            using (var scope = _tracer.BuildSpan(actionName).StartActive(true)) {
                Exception fault = new System.Net.Sockets.SocketException(errorCode: 10013);

                var chaosPolicy = MonkeyPolicy.InjectExceptionAsync(with 
                => with.Fault(fault)
                    .InjectionRate(0.15)
                    .Enabled(true));
                var mix = Policy.WrapAsync(GetPolicy(), chaosPolicy);
                return await mix.ExecuteAsync((ct) => GetForecasts(ct), CancellationToken.None);
            }
        }

        private IAsyncPolicy GetPolicy() {
            // Fill inn answer by changing code from here
            var retries = 0;
            Program.ConfiguredRetries.Set(retries);
            Program.ConfiguredRetries.Publish();
            var policy = Policy.Handle<Exception>().RetryAsync(retries, (ex, attempt) => Program.ExecutedRetries.Inc());

            // to here, anything outside of that is cheating.
            // But cheating is encouraged as long as the rationale and code
            // is shared with the workshop :)
            // Also, if you cheat or add something fun, consider making a PR for a new 
            // challenge to the workshop!
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