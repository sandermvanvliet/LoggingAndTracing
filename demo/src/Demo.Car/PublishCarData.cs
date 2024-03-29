﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Demo.Middlewares;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;

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
            // Initialize new context
            CorrelationContext.Instance = new CorrelationContext();

            // Push correlation id onto log context
            using(LogContext.PushProperty("correlation_id", CorrelationContext.Instance.CorrelationId))
            {
                _logger.Information("Publishing car data for VIN: " + _configuration.Vin);

                var serializedContent = JsonConvert.SerializeObject(
                    new
                    {
                        Timestamp = DateTime.UtcNow,
                        BatteryPercentage = _random.Next(1, 100),
                        ChargeState = "Charging"
                    });

                var content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

                try
                {
                    var response = _httpClient
                        .PostAsync($"/api/cars/{_configuration.Vin}/data", content)
                        .GetAwaiter()
                        .GetResult();

                    if(!response.IsSuccessStatusCode)
                    {
                        _logger.Error("Publish failed because API returned status code " + response.StatusCode);
                    }
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
}