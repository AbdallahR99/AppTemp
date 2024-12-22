using AppTemp.Core.Persistence;
using AppTemp.Infrastructure.Persistence;
using AppTemp.Catalog.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using AppTemp.Infrastructure.Persistence;
using AppTemp.Core.Persistence;

namespace AppTemp.Catalog.Infrastructure.Persistence;

public sealed class CatalogDbContext : AppDbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options, IPublisher publisher, IOptions<DatabaseOptions> settings)
        : base(options, publisher, settings)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Brand> Brands { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        modelBuilder.HasDefaultSchema(SchemaNames.Catalog);
    }
}
