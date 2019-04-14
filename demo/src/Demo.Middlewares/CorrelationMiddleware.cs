using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Demo.Middlewares
{
    public class CorrelationMiddleware
    {
        private const string CorrelationIdHeaderName = "Demo-CorrelationId";
        private readonly RequestDelegate _next;

        public CorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            CorrelationContext.Instance = new CorrelationContext
            {
                CorrelationId = GetCorrelationIdFromRequest(context.Request)
            };

            // Properties are disposable so we can ensure that
            // we remove the property after the next middleware is handled.
            using (LogContext.PushProperty("correlation_id", CorrelationContext.Instance.CorrelationId))
            {
                await _next(context);
            }
        }

        private string GetCorrelationIdFromRequest(HttpRequest request)
        {
            if (request.Headers.Any(header => header.Key == CorrelationIdHeaderName))
            {
                var value = request.Headers[CorrelationIdHeaderName].FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            // If there was no header present or its value was empty
            // we can use a new identifier
            return Guid.NewGuid().ToString("D");
        }
    }
}