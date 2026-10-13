namespace SmartGatewayApi.Services;

public interface IAuditService
{
    Task WriteAsync(HttpContext context);
}
