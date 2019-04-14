using System;
using Demo.Middlewares;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Demo.UserApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithCommonProperties()
                .WriteTo.Seq("http://seq:5341")
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Fatal)
                .CreateLogger();

            var host = new WebHostBuilder()
                .ConfigureAppConfiguration(c => c.AddEnvironmentVariables())
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(_ => _.ClearProviders())
                .UseStartup<Startup>()
                .UseKestrel()
                .Build();

            try
            {
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal("Unexpected exception: " + ex.Message);
                Environment.ExitCode = 1;
            }
        }

        private static void ConfigureServices(WebHostBuilderContext context, IServiceCollection serviceCollection)
        {
            var configuration = new Configuration();
            context.Configuration.GetSection("UserApi").Bind(configuration);

            serviceCollection.AddSingleton(Log.Logger);
            serviceCollection.AddSingleton(configuration);
        }
    }
}