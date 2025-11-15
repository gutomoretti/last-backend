using Lastlink.Domain.Events;

namespace Lastlink.Application.Abstractions.Messaging;

public interface IProductEventPublisher
{
    Task PublishProductCreatedAsync(ProductCreatedDomainEvent domainEvent, CancellationToken cancellationToken);
}
