
namespace Real_Time_Chat_Backend.MiddleWares
{
    public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<LoggingMiddleware> _logger = logger;
        public async Task Invoke(HttpContext httpContext)
        {
            var path = httpContext.Request.Path.Value;

            // Skip logging for Kubernetes health checks
            if (path.StartsWith("/ready", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/healthz", StringComparison.OrdinalIgnoreCase))
            {
                await _next(httpContext);
                return;
            }

            _logger.LogInformation("Request: {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
            await _next(httpContext);
            _logger.LogInformation("Response: {StatusCode}", httpContext.Response.StatusCode);
        }
    }
}
