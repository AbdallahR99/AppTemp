
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

namespace AppTemp.Catalog.Application.Brands.v1;

public static class SearchBrand
{
    public class Command : PaginationFilter, IRequest<PagedList<Response>>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public sealed record Response(Guid? Id, string Name, string? Description);

    public sealed class Handler(
    [FromKeyedServices("catalog:brands")] IReadRepository<Brand> repository)
    : IRequestHandler<Command, PagedList<Response>>
    {
        public async Task<PagedList<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var spec = new SearchBrandSpecs(request);

            var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
            var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

            return new PagedList<Response>(items, request!.PageNumber, request!.PageSize, totalCount);
        }
    }
    public class SearchBrandSpecs : EntitiesByPaginationFilterSpec<Brand, Response>
    {
        public SearchBrandSpecs(Command command)
            : base(command) =>
            Query
                .OrderBy(c => c.Name, !command.HasOrderBy())
                .Where(b => b.Name.Contains(command.Keyword), !string.IsNullOrEmpty(command.Keyword));
    }

    public static RouteHandlerBuilder Endpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] Command command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchBrand))
            .WithSummary("Gets a list of brands")
            .WithDescription("Gets a list of brands with pagination and filtering support")
            .Produces<PagedList<Response>>()
            .RequirePermission("Permissions.Brands.View")
            .MapToApiVersion(1);
    }

}
