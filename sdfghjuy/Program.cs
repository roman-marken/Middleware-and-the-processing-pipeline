using SmartGatewayApi.Middleware;
using SmartGatewayApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<FactoryBasedAuditMiddleware>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseSecurityHeaders();
app.UseCorrelationId();
app.UseMaintenanceMode();
app.UseIpRateLimiter();
app.UseRequestTiming();
app.UseFactoryAudit();

app.UseHttpsRedirection();
app.UseRouting();

app.UseDocsSecurity();
app.MapOpenApi();
app.MapScalarApiReference();

app.MapControllers();

app.Run();
