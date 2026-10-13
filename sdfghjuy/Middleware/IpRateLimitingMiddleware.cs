using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace SmartGatewayApi.Middleware;

public class IpRateLimitingMiddleware
{
    private static readonly ConcurrentDictionary<string, RateLimitInfo> Requests = new();

    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public IpRateLimitingMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var maxRequests = _configuration.GetValue<int>("RateLimit:MaxRequests", 10);
        var windowSeconds = _configuration.GetValue<int>("RateLimit:WindowSeconds", 60);
        var now = DateTime.UtcNow;

        var info = Requests.AddOrUpdate(
            ip,
            _ => new RateLimitInfo(1, now),
            (_, old) =>
            {
                if ((now - old.WindowStart).TotalSeconds >= windowSeconds)
                {
                    return new RateLimitInfo(1, now);
                }

                return old with { Count = old.Count + 1 };
            });

        if (info.Count > maxRequests)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers["Retry-After"] = windowSeconds.ToString();

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status429TooManyRequests,
                Title = "Too Many Requests",
                Detail = "Too many requests from your IP address."
            };

            await context.Response.WriteAsJsonAsync(problem);
            return;
        }

        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-RateLimit-Remaining"] = (maxRequests - info.Count).ToString();
            return Task.CompletedTask;
        });

        await _next(context);
    }

    private record RateLimitInfo(int Count, DateTime WindowStart);
}
