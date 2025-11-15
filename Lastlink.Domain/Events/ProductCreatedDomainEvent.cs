using Lastlink.Domain.Abstractions;

namespace Lastlink.Domain.Events;

public sealed class ProductCreatedDomainEvent : IDomainEvent
{
    public ProductCreatedDomainEvent(Guid productId, string name, string category, decimal unitCost, DateTime occurredOn)
    {
        ProductId = productId;
        Name = name;
        Category = category;
        UnitCost = unitCost;
        OccurredOn = occurredOn;
    }

    public Guid ProductId { get; }
    public string Name { get; }
    public string Category { get; }
    public decimal UnitCost { get; }
    public DateTime OccurredOn { get; }
}
