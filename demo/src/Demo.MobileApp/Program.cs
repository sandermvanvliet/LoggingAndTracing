﻿using System;
using Demo.Middlewares;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Demo.MobileApp
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
            context.Configuration.GetSection("MobileApp").Bind(configuration);

            serviceCollection.AddHostedService<CallMobileApi>();
            serviceCollection.AddHostedService<CallSummary>();

            serviceCollection.AddSingleton(Log.Logger);
            serviceCollection.AddSingleton(configuration);

            serviceCollection.AddTransient<CorrelationMessageHandler>();

            serviceCollection
                .AddHttpClient("mobile-api", _ =>
                {
                    Log.Logger.Information("Using MobileApiUrl: " + configuration.MobileApiUrl);
                    _.BaseAddress = new Uri(configuration.MobileApiUrl);
                    _.Timeout = TimeSpan.FromSeconds(1);
                })
                .AddHttpMessageHandler<CorrelationMessageHandler>();
        }
    }
}
