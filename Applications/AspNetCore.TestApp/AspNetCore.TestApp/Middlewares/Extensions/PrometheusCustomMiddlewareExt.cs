using Microsoft.AspNetCore.Builder;

namespace AspNetCore.TestApp.Middlewares.Extensions
{
    public static class PrometheusCustomMiddlewareExt
    {
        public static IApplicationBuilder UsePrometheusCustomMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PrometheusCustomMiddleware>();
        }
    }
}
