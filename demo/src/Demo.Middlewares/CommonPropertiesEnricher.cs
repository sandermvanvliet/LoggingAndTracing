using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Demo.Middlewares
{
    internal class CommonPropertiesEnricher : ILogEventEnricher
    {
        private readonly string _applicationName;
        private readonly string _applicationInstance;

        public CommonPropertiesEnricher()
        {
            _applicationName = Environment.GetEnvironmentVariable("APP_NAME") ?? "unknown";
            _applicationInstance = Environment.GetEnvironmentVariable("APP_INSTANCE") ?? "unkown";
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("app_name", _applicationName));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("app_intance", _applicationInstance));
        }
    }

    public static class LoggerEnrichmentConfigurationExtensions
    {
        public static LoggerConfiguration WithCommonProperties(
            this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            return enrichmentConfiguration.With<CommonPropertiesEnricher>();
        }
    }
}