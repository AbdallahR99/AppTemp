using System.Security.Claims;
using AppTemp.Core.Auth.Shared;
using AppTemp.Core.Identity.Users.Abstractions;
using AppTemp.Core.Identity.Users.Features.UpdateUser;
using AppTemp.Infrastructure.Auth.Policy;
using AppTemp.Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AppTemp.Infrastructure.Identity.Users.Endpoints;
public static class UpdateUserEndpoint
{
    internal static RouteHandlerBuilder MapUpdateUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/profile", (UpdateUserCommand request, ISender mediator, ClaimsPrincipal user, IUserService service) =>
        {
            if (user.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException();
            }
            return service.UpdateAsync(request, userId);
        })
        .WithName(nameof(UpdateUserEndpoint))
        .WithSummary("update user profile")
        .RequirePermission("Permissions.Users.Update")
        .WithDescription("update user profile");
    }
}
