﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Demo.Middlewares;
using Serilog.Events;

namespace Demo.Car
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithCommonProperties()
                .WriteTo.Seq("http://seq:5341")
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Fatal)
                .CreateLogger();

            var host = new HostBuilder()
                .ConfigureAppConfiguration(c => c.AddEnvironmentVariables())
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(_ => _.ClearProviders())
                .RunConsoleAsync();

            try
            {
                host
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal("Unexpected exception: " + ex.Message);
                Environment.ExitCode = 1;
            }
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection serviceCollection)
        {
            var configuration = new Configuration();
            context.Configuration.GetSection("CarApp").Bind(configuration);

            serviceCollection.AddHostedService<PublishCarData>();

            serviceCollection.AddSingleton(Log.Logger);
            serviceCollection.AddSingleton(configuration);
            
            serviceCollection.AddTransient<CorrelationMessageHandler>();

            serviceCollection
                .AddHttpClient("car-api", _ =>
                {
                    Log.Logger.Information("Using CarApiUrl: " + configuration.CarApiUrl);
                    _.BaseAddress = new Uri(configuration.CarApiUrl);
                    _.Timeout = TimeSpan.FromSeconds(1);
                })
                .AddHttpMessageHandler<CorrelationMessageHandler>();
        }
    }
}
