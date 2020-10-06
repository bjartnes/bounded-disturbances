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
using Polly.Contrib.Simmy.Outcomes;
using System.Net.Http;

namespace api_under_test.Controllers
{
    [ApiController]
    [Route("weatherforecast_challenge7")]
    public class Challenge7Controller: ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Challenge7Controller> _logger;
        private readonly Random _rng; 
        private readonly HttpClient _client; 

        public Challenge7Controller(ILogger<Challenge7Controller> logger)
        {
            _rng = new Random();
            _logger = logger;
            _client = new HttpClient();
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var policy = GetPolicy();
            // these lines might need to be moved around
            var url = new Uri(@"https://localhost:5001/weatherforecast_challenge1");
            var msg = new HttpRequestMessage(HttpMethod.Get, url); 
            // to somewhere... and the GetForecasts signature got to loose its message
            return await policy.ExecuteAsync(() => GetForecasts(msg)); 
        }

        private IAsyncPolicy GetPolicy() {
            return Policy.Handle<Exception>().RetryAsync(3);
        }

        private async Task<string> GetForecasts(HttpRequestMessage msg)
        {
               var resp = await _client.SendAsync(msg);
               if (_rng.NextDouble() > 0.95) {
                    throw new Exception( "oops");
               }

               return await resp.Content.ReadAsStringAsync(); 
        } 
    } 
} 
