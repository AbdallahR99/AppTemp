using AppTemp.Core.Domain.Events;

namespace AppTemp.Catalog.Domain.Events;
public sealed record ProductUpdated : DomainEvent
{
    public Product? Product { get; set; }
}
