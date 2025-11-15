namespace Lastlink.Infrastructure.Messaging;

public sealed record ProductCreatedMessage(Guid ProductId, string Name, string Category, decimal UnitCost, DateTime OccurredOn);
