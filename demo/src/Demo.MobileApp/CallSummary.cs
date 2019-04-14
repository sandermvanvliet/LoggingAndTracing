using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace Demo.MobileApp
{
    public class CallSummary : IHostedService
    {
        private const int MinimumInterval = 1; // Seconds
        private const int MaximumInterval = 4; // Seconds
        private readonly ILogger _logger;
        private readonly Configuration _configuration;
        private readonly Random _random;
        private readonly Timer _timer;
        private HttpClient _httpClient;

        public CallSummary(ILogger logger, Configuration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _timer = new Timer(_ => { CallApi(); });
            _random = new Random(1);
            _httpClient = httpClientFactory.CreateClient("mobile-api");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ScheduleNextInterval();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(-1, Timeout.Infinite);

            return Task.CompletedTask;
        }

        private void ScheduleNextInterval()
        {
            var randomValue = _random.Next(MinimumInterval, MaximumInterval);

            _timer.Change(TimeSpan.FromSeconds(randomValue), Timeout.InfiniteTimeSpan);
        }

        private void CallApi()
        {
            _logger.Information("Calling Mobile API for summary");

            try
            {
                var response = _httpClient
                    .GetAsync($"/api/summary/")
                    .GetAwaiter()
                    .GetResult();

                if(response.IsSuccessStatusCode)
                {
                    var summary = JsonConvert.DeserializeObject<Summary>(
                        response
                        .Content
                        .ReadAsStringAsync()
                        .GetAwaiter()
                        .GetResult());
                    
                    _logger.Information("Got data " + summary.Name + ", " + summary.Id + ", " + summary.Vin);
                }
                else {
                    _logger.Error("Mobile API responded with " + response.StatusCode);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.Error("Call timed out");
            }
            catch (Exception ex)
            {
                _logger.Error("Call failed because " + ex.Message);
            }

            ScheduleNextInterval();
        }
    }
}