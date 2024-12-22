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

namespace AppTemp.Catalog.Application.Products.v1;

public static class UpdateProduct
{
    public sealed record Command(
    Guid Id,
    string? Name,
    decimal Price,
    string? Description = null,
    Guid? BrandId = null) : IRequest<UpdateProductResponse>;
    public sealed record UpdateProductResponse(Guid? Id);

    public class UpdateProductCommandValidator : AbstractValidator<Command>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
            RuleFor(p => p.Price).GreaterThan(0);
        }
    }


    public sealed class Handler(
    ILogger<Handler> logger,
    [FromKeyedServices("catalog:products")] IRepository<Product> repository)
    : IRequestHandler<Command, UpdateProductResponse>
    {
        public async Task<UpdateProductResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var product = await repository.GetByIdAsync(request.Id, cancellationToken);
            _ = product ?? throw new ProductNotFoundException(request.Id);
            var updatedProduct = product.Update(request.Name, request.Description, request.Price, request.BrandId);
            await repository.UpdateAsync(updatedProduct, cancellationToken);
            logger.LogInformation("product with id : {ProductId} updated.", product.Id);
            return new UpdateProductResponse(product.Id);
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
            .WithName(nameof(UpdateProduct))
            .WithSummary("update a product")
            .WithDescription("update a product")
            .Produces<UpdateProductResponse>()
            .RequirePermission("Permissions.Products.Update")
            .MapToApiVersion(1);
    }
}
