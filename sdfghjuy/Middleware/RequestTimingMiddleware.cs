using System.Diagnostics;

namespace SmartGatewayApi.Middleware;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        context.Response.OnStarting(() =>
        {
            var ms = stopwatch.ElapsedMilliseconds;
            context.Response.Headers["X-Response-Time-Ms"] = $"{ms}ms";
            return Task.CompletedTask;
        });

        await _next(context);

        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > 500)
        {
            _logger.LogWarning(
                "⚠️ [SLOW REQUEST] {Method} {Path} виконувався {ElapsedMs} ms",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
