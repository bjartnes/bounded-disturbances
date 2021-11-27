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
using OpenTracing;
using Unleash;
using System.Net.Http;

namespace api_under_test.Controllers
{
    [ApiController]
    [Route("weatherforecast_challenge20")]
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
        public async Task<string> Get()
        {
            var actionName = ControllerContext.ActionDescriptor.DisplayName;
            using (var scope = _tracer.BuildSpan(actionName).StartActive(true)) {
                var chaosPolicy = MonkeyPolicy.InjectLatencyAsync(with =>
                 with.Latency(TimeSpan.FromSeconds(30))
                    .InjectionRate(0.85)
                    .Enabled(false));
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
            return policy; 
        }
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private Task<string> GetForecasts(CancellationToken ct)
        {
            var fooToggle = _unleash.IsEnabled("foo");
            return fooToggle ? Task.FromResult(RandomString(43000)): Task.FromResult(RandomString(42000));
        }
    }
}