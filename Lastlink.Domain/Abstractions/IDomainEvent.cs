namespace Lastlink.Domain.Abstractions;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
