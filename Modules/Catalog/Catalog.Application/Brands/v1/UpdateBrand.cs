using AppTemp.Catalog.Domain.Exceptions;
using AppTemp.Catalog.Domain;
using AppTemp.Core.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using AppTemp.Infrastructure.Auth.Policy;

namespace AppTemp.Catalog.Application.Brands.v1;

public static class UpdateBrand
{
    public sealed record Command(
    Guid Id,
    string? Name,
    string? Description = null) : IRequest<Response>;
    public sealed record Response(Guid? Id);

    public class UpdateBrandCommandValidator : AbstractValidator<Command>
    {
        public UpdateBrandCommandValidator()
        {
            RuleFor(b => b.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
            RuleFor(b => b.Description).MaximumLength(1000);
        }
    }

    public sealed class Handler(
    ILogger<Handler> logger,
    [FromKeyedServices("catalog:brands")] IRepository<Brand> repository)
    : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var brand = await repository.GetByIdAsync(request.Id, cancellationToken);
            _ = brand ?? throw new BrandNotFoundException(request.Id);
            var updatedBrand = brand.Update(request.Name, request.Description);
            await repository.UpdateAsync(updatedBrand, cancellationToken);
            logger.LogInformation("Brand with id : {BrandId} updated.", brand.Id);
            return new Response(brand.Id);
        }
    }
    public static RouteHandlerBuilder Endpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, Command request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateBrand))
            .WithSummary("update a brand")
            .WithDescription("update a brand")
            .Produces<Response>()
            .RequirePermission("Permissions.Brands.Update")
            .MapToApiVersion(1);
    }


}
