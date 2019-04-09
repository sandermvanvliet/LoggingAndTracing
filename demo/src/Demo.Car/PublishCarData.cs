using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace Demo.Car
{
    public class PublishCarData : IHostedService
    {
        private const int MinimumInterval = 1; // Seconds
        private const int MaximumInterval = 4; // Seconds
        private readonly ILogger _logger;
        private readonly Configuration _configuration;
        private readonly Random _random;
        private readonly Timer _timer;
        private HttpClient _httpClient;

        public PublishCarData(ILogger logger, Configuration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _timer = new Timer(_ => { Publish(); });
            _random = new Random(1);
            _httpClient = httpClientFactory.CreateClient("car-api");
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

        private void Publish()
        {
            _logger.Information("Publishing car data for VIN: " + _configuration.Vin);

            var serializedContent = JsonConvert.SerializeObject(
                new
                {
                    Timestamp = DateTime.UtcNow,
                    ChargeState = _random.Next(1, 100)
                });

            var content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            try
            {
                _httpClient
                    .PostAsync($"/api/vehicles/{_configuration.Vin}/data", content)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (OperationCanceledException)
            {
                _logger.Error("Publish timed out");
            }
            catch (Exception ex)
            {
                _logger.Error("Publish failed because " + ex.Message);
            }

            ScheduleNextInterval();
        }
    }
}