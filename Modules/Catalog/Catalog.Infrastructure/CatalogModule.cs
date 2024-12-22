using AppTemp.Catalog.Application.Brands.v1;
using AppTemp.Catalog.Application.Products.v1;
using AppTemp.Catalog.Domain;
using AppTemp.Catalog.Infrastructure.Persistence;
using AppTemp.Core.Persistence;
using AppTemp.Infrastructure.Persistence;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AppTemp.Catalog.Infrastructure;

public static class CatalogModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("catalog") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var productGroup = app.MapGroup("products").WithTags("products");
            CreateProduct.Endpoint(productGroup);
            GetProduct.Endpoint(productGroup);
            DeleteProduct.Endpoint(productGroup);
            SearchProduct.Endpoint(productGroup);
            UpdateProduct.Endpoint(productGroup);

            //productGroup.MapProductCreationEndpoint();
            //productGroup.MapGetProductEndpoint();
            //productGroup.MapGetProductListEndpoint();
            //productGroup.MapProductUpdateEndpoint();
            //productGroup.MapProductDeleteEndpoint();

            var brandGroup = app.MapGroup("brands").WithTags("brands");
            CreateBrand.Endpoint(brandGroup);
            GetBrand.Endpoint(brandGroup);
            DeleteBrand.Endpoint(brandGroup);
            SearchBrand.Endpoint(brandGroup);
            UpdateBrand.Endpoint(brandGroup);


            //brandGroup.MapBrandCreationEndpoint();
            //brandGroup.MapGetBrandEndpoint();
            //brandGroup.MapGetBrandListEndpoint();
            //brandGroup.MapBrandUpdateEndpoint();
            //brandGroup.MapBrandDeleteEndpoint();
        }
    }
    public static WebApplicationBuilder RegisterCatalogServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<CatalogDbContext>();
        builder.Services.AddScoped<IDbInitializer, CatalogDbInitializer>();
        builder.Services.AddKeyedScoped<IRepository<Product>, CatalogRepository<Product>>("catalog:products");
        builder.Services.AddKeyedScoped<IReadRepository<Product>, CatalogRepository<Product>>("catalog:products");
        builder.Services.AddKeyedScoped<IRepository<Brand>, CatalogRepository<Brand>>("catalog:brands");
        builder.Services.AddKeyedScoped<IReadRepository<Brand>, CatalogRepository<Brand>>("catalog:brands");
        return builder;
    }
    public static WebApplication UseCatalogModule(this WebApplication app)
    {
        return app;
    }
}