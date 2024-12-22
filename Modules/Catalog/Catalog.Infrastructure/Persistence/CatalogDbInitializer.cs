using AppTemp.Core.Persistence;
using AppTemp.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppTemp.Catalog.Infrastructure.Persistence;
internal sealed class CatalogDbInitializer(
    ILogger<CatalogDbInitializer> logger,
    CatalogDbContext context) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[{ContextId}] applied database migrations for catalog module", context.ContextId);
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        const string Name = "Keychron V6 QMK Custom Wired Mechanical Keyboard";
        const string Description = "A full-size layout QMK/VIA custom mechanical keyboard";
        const decimal Price = 79;
        Guid? BrandId = null;
        if (await context.Products.FirstOrDefaultAsync(t => t.Name == Name, cancellationToken).ConfigureAwait(false) is null)
        {
            var product = Product.Create(Name, Description, Price, BrandId);
            await context.Products.AddAsync(product, cancellationToken);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[{ContextId}] seeding default catalog data", context.ContextId);
        }
    }
}
