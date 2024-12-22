using AppTemp.Core.Domain.Events;

namespace AppTemp.Catalog.Domain.Events;
public sealed record BrandCreated : DomainEvent
{
    public Brand? Brand { get; set; }
}
