using Lastlink.Domain.Abstractions;
using Lastlink.Domain.Events;

namespace Lastlink.Domain.Entities;

public sealed class Product
{
    private readonly List<IDomainEvent> _domainEvents = new();

    private Product()
    {
    }

    private Product(Guid id, string name, string category, decimal unitCost, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Category = category;
        UnitCost = unitCost;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal UnitCost { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public static Product Create(string name, string category, decimal unitCost)
    {
        var now = DateTime.UtcNow;
        var product = new Product(Guid.NewGuid(), name, category, unitCost, now);

        product.AddDomainEvent(new ProductCreatedDomainEvent(product.Id, product.Name, product.Category, product.UnitCost, now));

        return product;
    }

    public void Update(string name, string category, decimal unitCost)
    {
        Name = name;
        Category = category;
        UnitCost = unitCost;
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
