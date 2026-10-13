using SmartGatewayApi.Services;

namespace SmartGatewayApi.Middleware;

public class FactoryBasedAuditMiddleware : IMiddleware
{
    private readonly IAuditService _auditService;

    public FactoryBasedAuditMiddleware(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await _auditService.WriteAsync(context);
        await next(context);
    }
}
