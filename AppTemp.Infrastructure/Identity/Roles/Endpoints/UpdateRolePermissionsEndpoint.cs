using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using AppTemp.Core.Identity.Roles;
using AppTemp.Core.Identity.Roles.Features.UpdatePermissions;
using AppTemp.Infrastructure.Auth.Policy;

namespace AppTemp.Infrastructure.Identity.Roles.Endpoints;
public static class UpdateRolePermissionsEndpoint
{
    public static RouteHandlerBuilder MapUpdateRolePermissionsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/{id}/permissions", async (
            UpdatePermissionsCommand request,
            IRoleService roleService,
            string id,
            [FromServices] IValidator<UpdatePermissionsCommand> validator) =>
        {
            if (id != request.RoleId) return Results.BadRequest();
            var response = await roleService.UpdatePermissionsAsync(request);
            return Results.Ok(response);
        })
        .WithName(nameof(UpdateRolePermissionsEndpoint))
        .WithSummary("update role permissions")
        .RequirePermission("Permissions.Roles.Create")
        .WithDescription("update role permissions");
    }
}
