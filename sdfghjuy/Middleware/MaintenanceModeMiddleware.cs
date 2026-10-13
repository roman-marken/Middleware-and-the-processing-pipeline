using Microsoft.AspNetCore.Mvc;

namespace SmartGatewayApi.Middleware;

public class MaintenanceModeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public MaintenanceModeMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var enabled = _configuration.GetValue<bool>("MaintenanceMode:Enabled");

        if (!enabled || IsHealthCheck(context))
        {
            await _next(context);
            return;
        }

        var retryAfter = _configuration.GetValue<int>("MaintenanceMode:RetryAfterSeconds", 300);

        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        context.Response.Headers["Retry-After"] = retryAfter.ToString();

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status503ServiceUnavailable,
            Title = "Service Unavailable",
            Detail = "API is in maintenance mode now."
        };

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static bool IsHealthCheck(HttpContext context)
    {
        return context.Request.Method == HttpMethods.Get
            && context.Request.Path.Equals("/api/health", StringComparison.OrdinalIgnoreCase);
    }
}
