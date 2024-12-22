using AppTemp.Core.Domain;
using AppTemp.Core.Domain.Contracts;
using AppTemp.Catalog.Domain.Events;

namespace AppTemp.Catalog.Domain;
public class Brand : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    public static Brand Create(string name, string? description)
    {
        var brand = new Brand
        {
            Name = name,
            Description = description
        };

        brand.AddDomainEvent(new BrandCreated() { Brand = brand });

        return brand;
    }

    public Brand Update(string? name, string? description)
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;

        AddDomainEvent(new BrandUpdated() { Brand = this });

        return this;
    }

    public static Brand Update(Guid id, string name, string? description)
    {
        var brand = new Brand
        {
            Id = id,
            Name = name,
            Description = description
        };

        brand.AddDomainEvent(new BrandUpdated() { Brand = brand });

        return brand;
    }
}

