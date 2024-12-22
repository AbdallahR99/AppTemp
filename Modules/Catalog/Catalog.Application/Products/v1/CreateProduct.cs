using AppTemp.Catalog.Domain;
using AppTemp.Catalog.Domain.Events;
using AppTemp.Core.Persistence;
using AppTemp.Infrastructure.Auth.Policy;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;


namespace AppTemp.Catalog.Application.Products.v1;

public static class CreateProduct
{
    public sealed record Command(
    [property: DefaultValue("Sample Product")] string? Name,
    [property: DefaultValue(10)] decimal Price,
    [property: DefaultValue("Descriptive Description")] string? Description = null,
    [property: DefaultValue(null)] Guid? BrandId = null) : IRequest<Response>;

    public sealed record Response(Guid? Id);

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
            RuleFor(p => p.Price).GreaterThan(0);
        }
    }

    public sealed class Handler(
    ILogger<Handler> logger,
    [FromKeyedServices("catalog:products")] IRepository<Product> repository)
    : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var product = Product.Create(request.Name!, request.Description, request.Price, request.BrandId);
            await repository.AddAsync(product, cancellationToken);
            logger.LogInformation("product created {ProductId}", product.Id);
            return new Response(product.Id);
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
            .WithName(nameof(CreateProduct))
            .WithSummary("creates a product")
            .WithDescription("creates a product")
            .Produces<Response>()
            .RequirePermission("Permissions.Products.Create")
            .MapToApiVersion(1);
    }
    public class CreatedEventHandler(ILogger<CreatedEventHandler> logger) : INotificationHandler<ProductCreated>
    {
        public async Task Handle(ProductCreated notification,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("handling product created domain event..");
            await Task.FromResult(notification);
            logger.LogInformation("finished handling product created domain event..");
        }
    }



}
