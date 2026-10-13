using Microsoft.AspNetCore.Mvc;

namespace SmartGatewayApi.Middleware;

public class DocsSecurityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public DocsSecurityMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var enabled = _configuration.GetValue<bool>("DocsSecurity:Enabled", true);

        if (!enabled || !IsDocsPath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var correctKey = _configuration["DocsSecurity:AccessKey"];
        var key = GetKey(context);

        if (key == correctKey)
        {
            await _next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status403Forbidden;

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden",
            Detail = "Docs access key is missing or invalid."
        };

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static bool IsDocsPath(PathString path)
    {
        return path.StartsWithSegments("/openapi")
            || path.StartsWithSegments("/scalar")
            || path.StartsWithSegments("/swagger");
    }

    private static string? GetKey(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Docs-Key", out var headerKey))
        {
            return headerKey.FirstOrDefault();
        }

        if (context.Request.Query.TryGetValue("key", out var queryKey))
        {
            return queryKey.FirstOrDefault();
        }

        if (context.Request.Query.TryGetValue("docsKey", out var docsKey))
        {
            return docsKey.FirstOrDefault();
        }

        return null;
    }
}
