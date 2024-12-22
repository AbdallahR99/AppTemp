using AppTemp.Catalog.Domain.Exceptions;
using AppTemp.Catalog.Domain;
using AppTemp.Core.Persistence;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using AppTemp.Infrastructure.Auth.Policy;

namespace AppTemp.Catalog.Application.Brands.v1;

public static class DeleteBrand
{
    public sealed record Command(
    Guid Id) : IRequest;


    public sealed class Handler(
    ILogger<Handler> logger,
    [FromKeyedServices("catalog:brands")] IRepository<Brand> repository)
    : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var brand = await repository.GetByIdAsync(request.Id, cancellationToken);
            _ = brand ?? throw new BrandNotFoundException(request.Id);
            await repository.DeleteAsync(brand, cancellationToken);
            logger.LogInformation("Brand with id : {BrandId} deleted", brand.Id);
        }
    }

    public static RouteHandlerBuilder Endpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                await mediator.Send(new Command(id));
                return Results.NoContent();
            })
            .WithName(nameof(DeleteBrand))
            .WithSummary("deletes brand by id")
            .WithDescription("deletes brand by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.Brands.Delete")
            .MapToApiVersion(1);
    }


}
