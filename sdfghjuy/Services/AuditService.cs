namespace SmartGatewayApi.Services;

public class AuditService : IAuditService
{
    private readonly ILogger<AuditService> _logger;

    public AuditService(ILogger<AuditService> logger)
    {
        _logger = logger;
    }

    public Task WriteAsync(HttpContext context)
    {
        _logger.LogInformation("Audit: {Method} {Path}", context.Request.Method, context.Request.Path);
        return Task.CompletedTask;
    }
}
