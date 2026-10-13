namespace SmartGatewayApi.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        => app.UseMiddleware<SecurityHeadersMiddleware>();

    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        => app.UseMiddleware<CorrelationIdMiddleware>();

    public static IApplicationBuilder UseMaintenanceMode(this IApplicationBuilder app)
        => app.UseMiddleware<MaintenanceModeMiddleware>();

    public static IApplicationBuilder UseIpRateLimiter(this IApplicationBuilder app)
        => app.UseMiddleware<IpRateLimitingMiddleware>();

    public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder app)
        => app.UseMiddleware<RequestTimingMiddleware>();

    public static IApplicationBuilder UseFactoryAudit(this IApplicationBuilder app)
        => app.UseMiddleware<FactoryBasedAuditMiddleware>();

    public static IApplicationBuilder UseDocsSecurity(this IApplicationBuilder app)
        => app.UseMiddleware<DocsSecurityMiddleware>();

    public static IEndpointRouteBuilder MapOpenApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/openapi/v1.json", () => Results.Json(new
        {
            openapi = "3.0.1",
            info = new { title = "SmartGatewayApi", version = "v1" },
            paths = new { }
        }));

        return app;
    }

    public static IEndpointRouteBuilder MapScalarApiReference(this IEndpointRouteBuilder app)
    {
        app.MapGet("/scalar/v1", () => Results.Content("""
            <!doctype html>
            <html>
            <head><title>SmartGatewayApi Docs</title></head>
            <body>
                <h1>SmartGatewayApi Docs</h1>
                <p>Simple documentation page for homework.</p>
                <a href="/openapi/v1.json">OpenAPI JSON</a>
            </body>
            </html>
            """, "text/html"));

        return app;
    }
}
