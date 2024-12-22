using AppTemp.Catalog.Domain;
using AppTemp.Catalog.Domain.Events;
using AppTemp.Core.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using AppTemp.Infrastructure.Auth.Policy;

namespace AppTemp.Catalog.Application.Brands.v1;

public static class CreateBrand
{
    public sealed record Command(
    [property: DefaultValue("Sample Brand")] string? Name,
    [property: DefaultValue("Descriptive Description")] string? Description = null) : IRequest<Response>;

    public sealed record Response(Guid? Id);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
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
            var brand = Brand.Create(request.Name!, request.Description);
            await repository.AddAsync(brand, cancellationToken);
            logger.LogInformation("brand created {BrandId}", brand.Id);
            return new Response(brand.Id);
        }
    }




    public sealed class CreatedEventHandler(ILogger<CreatedEventHandler> logger) : INotificationHandler<BrandCreated>
    {
        public async Task Handle(BrandCreated notification,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("handling brand created domain event..");
            await Task.FromResult(notification);
            logger.LogInformation("finished handling brand created domain event..");
        }
    }

    public static RouteHandlerBuilder Endpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (Command request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateBrand))
            .WithSummary("creates a brand")
            .WithDescription("creates a brand")
            .Produces<Response>()
            .RequirePermission("Permissions.Brands.Create")
            .MapToApiVersion(1);
    }


}
