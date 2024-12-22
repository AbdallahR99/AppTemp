using AppTemp.Catalog.Domain;
using AppTemp.Catalog.Domain.Exceptions;
using AppTemp.Core.Caching;
using AppTemp.Core.Persistence;
using AppTemp.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTemp.Catalog.Application.Brands.v1;

public static class GetBrand
{
    public class Request : IRequest<Response>
    {
        public Guid Id { get; set; }
        public Request(Guid id) => Id = id;
    }

    public sealed record Response(Guid? Id, string Name, string? Description);
    public sealed class Handler(
    [FromKeyedServices("catalog:brands")] IReadRepository<Brand> repository,
    ICacheService cache)
    : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var item = await cache.GetOrSetAsync(
                $"brand:{request.Id}",
                async () =>
                {
                    var brandItem = await repository.GetByIdAsync(request.Id, cancellationToken);
                    if (brandItem == null) throw new BrandNotFoundException(request.Id);
                    return new Response(brandItem.Id, brandItem.Name, brandItem.Description);
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
            .WithName(nameof(GetBrand))
            .WithSummary("gets brand by id")
            .WithDescription("gets brand by id")
            .Produces<Response>()
            .RequirePermission("Permissions.Brands.View")
            .MapToApiVersion(1);
    }
}
