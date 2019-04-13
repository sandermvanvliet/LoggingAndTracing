using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Demo.MobileApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
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

            serviceCollection.AddHostedService<CallUserApi>();

            serviceCollection.AddSingleton(Log.Logger);
            serviceCollection.AddSingleton(configuration);

            serviceCollection.AddHttpClient("user-api", _ =>
            {
                Log.Logger.Information("Using UserApiUrl: " + configuration.UserApiUrl);
                _.BaseAddress = new Uri(configuration.UserApiUrl);
                _.Timeout = TimeSpan.FromSeconds(1);
            });
        }
    }
}
