
using AppTemp.Catalog.Domain;
using AppTemp.Core.Paging;
using AppTemp.Core.Persistence;
using AppTemp.Core.Specifications;
using AppTemp.Infrastructure.Auth.Policy;
using Ardalis.Specification;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AppTemp.Catalog.Application.Products.v1;

public static class SearchProduct
{

    public class Command : PaginationFilter, IRequest<PagedList<GetProduct.Response>>
    {
        public Guid? BrandId { get; set; }
        public decimal? MinimumRate { get; set; }
        public decimal? MaximumRate { get; set; }
    }


    public sealed class Handler(
    [FromKeyedServices("catalog:products")] IReadRepository<Product> repository)
    : IRequestHandler<Command, PagedList<GetProduct.Response>>
    {
        public async Task<PagedList<GetProduct.Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var spec = new Specs(request);

            var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
            var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

            return new PagedList<GetProduct.Response>(items, request!.PageNumber, request!.PageSize, totalCount);
        }
    }


    public class Specs : EntitiesByPaginationFilterSpec<Product, GetProduct.Response>
    {
        public Specs(Command command)
            : base(command) =>
            Query
                .Include(p => p.Brand)
                .OrderBy(c => c.Name, !command.HasOrderBy())
                .Where(p => p.BrandId == command.BrandId!.Value, command.BrandId.HasValue)
                .Where(p => p.Price >= command.MinimumRate!.Value, command.MinimumRate.HasValue)
                .Where(p => p.Price <= command.MaximumRate!.Value, command.MaximumRate.HasValue);
    }

    public static RouteHandlerBuilder Endpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] Command command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchProduct))
            .WithSummary("Gets a list of products")
            .WithDescription("Gets a list of products with pagination and filtering support")
            .Produces<PagedList<GetProduct.Response>>()
            .RequirePermission("Permissions.Products.View")
            .MapToApiVersion(1);
    }
}
