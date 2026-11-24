using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace LedgerAI.Api.Core.Security;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
    {
        var tenantIdText = context.Request.Headers["X-Tenant-Id"].FirstOrDefault()
                           ?? context.User.FindFirst("tenant_id")?.Value
                           ?? context.User.FindFirst("TenantId")?.Value;

        if (Guid.TryParse(tenantIdText, out var tenantId) && tenantId != Guid.Empty)
        {
            var tenantCode = context.User.FindFirst("tenant_code")?.Value;
            tenantContext.SetTenant(tenantId, tenantCode);
            context.Items["TenantId"] = tenantId;
        }

        await _next(context);
    }
}
