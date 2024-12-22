using AppTemp.Core.Identity.Tokens;
using AppTemp.Core.Identity.Tokens.Features.Refresh;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AppTemp.Infrastructure.Identity.Tokens.Endpoints;
public static class RefreshTokenEndpoint
{
    internal static RouteHandlerBuilder MapRefreshTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/refresh", (RefreshTokenCommand request,
            ITokenService service,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            string ip = context.GetIpAddress();
            return service.RefreshTokenAsync(request, ip!, cancellationToken);
        })
        .WithName(nameof(RefreshTokenEndpoint))
        .WithSummary("refresh JWTs")
        .WithDescription("refresh JWTs")
        .AllowAnonymous();
    }
}
