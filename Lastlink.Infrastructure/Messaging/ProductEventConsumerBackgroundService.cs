using System.Text;
using System.Text.Json;
using Lastlink.Domain.Entities;
using Lastlink.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lastlink.Infrastructure.Messaging;

public sealed class ProductEventConsumerBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<ProductEventConsumerBackgroundService> _logger;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private IConnection? _connection;
    private IModel? _channel;

    public ProductEventConsumerBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<ProductEventConsumerBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = CreateConnectionFactory();
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        DeclareMessagingInfrastructure(_channel);
        RegisterConsumer(_channel);

        _logger.LogInformation("RabbitMQ consumer inicializado.");

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();
        _connection?.Close();
        return base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }

    private void RegisterConsumer(IModel channel)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += HandleMessageAsync;

        channel.BasicConsume(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer);
    }

    private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        var payload = Encoding.UTF8.GetString(body);

        try
        {
            var message = JsonSerializer.Deserialize<ProductCreatedMessage>(payload, _serializerOptions);

            if (message is null)
            {
                _logger.LogWarning("Mensagem inválida recebida: {Payload}", payload);
                _channel?.BasicAck(eventArgs.DeliveryTag, multiple: false);
                return;
            }

            var productEvent = ProductEvent.FromAudit(message.ProductId, _options.RoutingKey, payload);

            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IProductEventRepository>();

            await repository.AddAsync(productEvent, CancellationToken.None);

            _channel?.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar mensagem RabbitMQ.");
            _channel?.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
        }
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
