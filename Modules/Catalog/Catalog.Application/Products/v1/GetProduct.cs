using AppTemp.Catalog.Application.Brands.v1;
using AppTemp.Catalog.Domain;
using AppTemp.Catalog.Domain.Exceptions;
using AppTemp.Core.Caching;
using AppTemp.Core.Persistence;
using AppTemp.Infrastructure.Auth.Policy;
using Ardalis.Specification;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;


namespace AppTemp.Catalog.Application.Products.v1;

public static class GetProduct
{
    public class Request : IRequest<Response>
    {
        public Guid Id { get; set; }
        public Request(Guid id) => Id = id;
    }
    public sealed record Response(Guid? Id, string Name, string? Description, decimal Price, GetBrand.Response? Brand);

    public class Specs : Specification<Product, Response>
    {
        public Specs(Guid id)
        {
            Query
                .Where(p => p.Id == id)
                .Include(p => p.Brand);
        }
    }

    public sealed class Handler(
    [FromKeyedServices("catalog:products")] IReadRepository<Product> repository,
    ICacheService cache)
    : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var item = await cache.GetOrSetAsync(
                $"product:{request.Id}",
                async () =>
                {
                    var spec = new Specs(request.Id);
                    var productItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                    if (productItem == null) throw new ProductNotFoundException(request.Id);
                    return productItem;
                },
                cancellationToken: cancellationToken);
            return item!;
        }
    }


    public static RouteHandlerBuilder Endpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new Request(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetProduct))
            .WithSummary("gets product by id")
            .WithDescription("gets prodct by id")
            .Produces<Response>()
            .RequirePermission("Permissions.Products.View")
            .MapToApiVersion(1);
    }

}
