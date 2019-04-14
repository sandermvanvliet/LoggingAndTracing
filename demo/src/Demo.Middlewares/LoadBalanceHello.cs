using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Demo.Middlewares
{
    public class LoadBalanceHello
    {
        private readonly RequestDelegate _next;

        public LoadBalanceHello(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if(context.Request.Path == "/loadbalance/hello")
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                return;
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}