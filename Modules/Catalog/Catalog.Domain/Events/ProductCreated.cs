using AppTemp.Core.Domain.Events;

namespace AppTemp.Catalog.Domain.Events;
public sealed record ProductCreated : DomainEvent
{
    public Product? Product { get; set; }
}
