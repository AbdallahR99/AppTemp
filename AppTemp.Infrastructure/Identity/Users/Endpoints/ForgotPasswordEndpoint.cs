using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using AppTemp.Core.Origin;
using AppTemp.Core.Identity.Users.Features.ForgotPassword;
using AppTemp.Core.Identity.Users.Abstractions;

namespace AppTemp.Infrastructure.Identity.Users.Endpoints;

public static class ForgotPasswordEndpoint
{
    internal static RouteHandlerBuilder MapForgotPasswordEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/forgot-password", async (HttpRequest request, string tenant, ForgotPasswordCommand command, IOptions<OriginOptions> settings, IValidator<ForgotPasswordCommand> validator, IUserService userService, CancellationToken cancellationToken) =>
        {
            ValidationResult result = await validator.ValidateAsync(command, cancellationToken);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }

            // Obtain origin from appsettings
            var origin = settings.Value;

            if (origin?.OriginUrl == null)
            {
                // Handle the case where OriginUrl is null
                return Results.BadRequest("Origin URL is not configured.");
            }

            await userService.ForgotPasswordAsync(command, origin.OriginUrl.ToString(), cancellationToken);
            return Results.Ok("Password reset email sent.");
        })
        .WithName(nameof(ForgotPasswordEndpoint))
        .WithSummary("Forgot password")
        .WithDescription("Generates a password reset token and sends it via email.")
        .AllowAnonymous();
    }

}
