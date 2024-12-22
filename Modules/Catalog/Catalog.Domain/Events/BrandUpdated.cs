using AppTemp.Core.Domain.Events;
using AppTemp.Catalog.Domain;

namespace AppTemp.Catalog.Domain.Events;
public sealed record BrandUpdated : DomainEvent
{
    public Brand? Brand { get; set; }
}
