using System.Text.Json;
using Lastlink.Application.Abstractions.Messaging;
using Lastlink.Domain.Events;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Lastlink.Infrastructure.Messaging;

public sealed class RabbitMqProductEventPublisher : IProductEventPublisher
{
    private readonly RabbitMqOptions _options;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public RabbitMqProductEventPublisher(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }

    public Task PublishProductCreatedAsync(ProductCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var factory = CreateConnectionFactory();

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        DeclareMessagingInfrastructure(channel);

        var message = new ProductCreatedMessage(domainEvent.ProductId, domainEvent.Name, domainEvent.Category, domainEvent.UnitCost, domainEvent.OccurredOn);
        var body = JsonSerializer.SerializeToUtf8Bytes(message, _serializerOptions);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        channel.BasicPublish(
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);

        return Task.CompletedTask;
    }

    private ConnectionFactory CreateConnectionFactory()
    {
        return new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            VirtualHost = _options.VirtualHost,
            UserName = _options.UserName,
            Password = _options.Password,
            DispatchConsumersAsync = true
        };
    }

    private void DeclareMessagingInfrastructure(IModel channel)
    {
        channel.ExchangeDeclare(
            exchange: _options.ExchangeName,
            type: ExchangeType.Direct,
            durable: true);

        channel.QueueDeclare(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        channel.QueueBind(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey);
    }
}
