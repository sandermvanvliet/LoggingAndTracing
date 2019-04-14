using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog.Context;

namespace Demo.Middlewares
{
    public class CorrelationMessageHandler : DelegatingHandler
    {
        private const string CorrelationIdHeaderName = "Demo-CorrelationId";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if(!request.Headers.Any(header => header.Key == CorrelationIdHeaderName))
            {
                if(CorrelationContext.Instance == null)
                {
                    CorrelationContext.Instance = new CorrelationContext();
                }

                request.Headers.Add(CorrelationIdHeaderName, CorrelationContext.Instance.CorrelationId.ToString("D"));
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}