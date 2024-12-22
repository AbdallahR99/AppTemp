﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using AppTemp.Core.Persistence;
using AppTemp.Core.Domain.Contracts;

namespace AppTemp.Infrastructure.Persistence;
public class AppDbContext(
    DbContextOptions options,
    IPublisher publisher,
    IOptions<DatabaseOptions> settings)
    : DbContext(options)
{
    private readonly IPublisher _publisher = publisher;
    private readonly DatabaseOptions _settings = settings.Value;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // QueryFilters need to be applied before base.OnModelCreating
        modelBuilder.AppendGlobalQueryFilter<ISoftDeletable>(s => s.Deleted == null);
        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();

        //if (!string.IsNullOrWhiteSpace(multiTenantContextAccessor?.MultiTenantContext.TenantInfo?.ConnectionString))
        //{
        //    optionsBuilder.ConfigureDatabase(_settings.Provider, multiTenantContextAccessor.MultiTenantContext.TenantInfo.ConnectionString!);
        //}
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        //this.TenantNotSetMode = TenantNotSetMode.Overwrite;
        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await PublishDomainEventsAsync().ConfigureAwait(false);
        return result;
    }
    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker.Entries<IEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .SelectMany(e =>
            {
                var domainEvents = e.DomainEvents.ToList();
                e.DomainEvents.Clear();
                return domainEvents;
            })
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent).ConfigureAwait(false);
        }
    }
}