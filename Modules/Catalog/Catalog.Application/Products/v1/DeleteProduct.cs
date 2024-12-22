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

namespace AppTemp.Catalog.Application.Products.v1;

public static class DeleteProduct
{
    public sealed record Command(
    Guid Id) : IRequest;

    public sealed class Handler(
    ILogger<Handler> logger,
    [FromKeyedServices("catalog:products")] IRepository<Product> repository)
    : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var product = await repository.GetByIdAsync(request.Id, cancellationToken);
            _ = product ?? throw new ProductNotFoundException(request.Id);
            await repository.DeleteAsync(product, cancellationToken);
            logger.LogInformation("product with id : {ProductId} deleted", product.Id);
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
            .WithName(nameof(DeleteProduct))
            .WithSummary("deletes product by id")
            .WithDescription("deletes product by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.Products.Delete")
            .MapToApiVersion(1);
    }
}
