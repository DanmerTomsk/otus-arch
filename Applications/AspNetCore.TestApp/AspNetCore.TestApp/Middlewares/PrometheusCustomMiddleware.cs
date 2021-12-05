using Microsoft.AspNetCore.Http;
using Npgsql;
using Prometheus;
using System.Threading.Tasks;

namespace AspNetCore.TestApp.Middlewares
{
    public class PrometheusCustomMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Histogram RequestDuration = Metrics
            .CreateHistogram("myapp_duration_seconds", "Histogram of all requests call processing durations.");

        public PrometheusCustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (RequestDuration.NewTimer())
            {
                await _next(context);
            }
        }
    }
}
