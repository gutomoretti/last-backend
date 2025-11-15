namespace Lastlink.Domain.Entities;

public sealed class ProductEvent
{
    private ProductEvent()
    {
    }

    private ProductEvent(Guid id, Guid productId, string eventType, string payload, DateTime createdAt)
    {
        Id = id;
        ProductId = productId;
        EventType = eventType;
        Payload = payload;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public static ProductEvent FromAudit(Guid productId, string eventType, string payload)
    {
        return new ProductEvent(Guid.NewGuid(), productId, eventType, payload, DateTime.UtcNow);
    }
}
