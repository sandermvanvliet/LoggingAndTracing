using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Demo.Middlewares;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;

namespace Demo.MobileApp
{
    public class CallMobileApi : IHostedService
    {
        private const int MinimumInterval = 1; // Seconds
        private const int MaximumInterval = 4; // Seconds
        private readonly ILogger _logger;
        private readonly Configuration _configuration;
        private readonly Random _random;
        private readonly Timer _timer;
        private HttpClient _httpClient;

        public CallMobileApi(ILogger logger, Configuration configuration, IHttpClientFactory httpClientFactory)
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
            // Initialize new context
            CorrelationContext.Instance = new CorrelationContext();

            // Push correlation id onto log context
            using(LogContext.PushProperty("correlation_id", CorrelationContext.Instance.CorrelationId))
            {
                _logger.Information("Calling Mobile API for user : " + _configuration.UserId);

                try
                {
                    var response = _httpClient
                        .GetAsync($"/api/users/{_configuration.UserId}")
                        .GetAwaiter()
                        .GetResult();

                    if(response.IsSuccessStatusCode)
                    {
                        var user = JsonConvert.DeserializeObject<User>(
                            response
                            .Content
                            .ReadAsStringAsync()
                            .GetAwaiter()
                            .GetResult());
                        
                        _logger.Information("Got user with name " + user.Name);
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
}