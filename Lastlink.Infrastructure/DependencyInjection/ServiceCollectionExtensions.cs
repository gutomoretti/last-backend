using Lastlink.Application.Abstractions.Messaging;
using Lastlink.Domain.Repositories;
using Lastlink.Infrastructure.Messaging;
using Lastlink.Infrastructure.Persistence;
using Lastlink.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lastlink.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' não configurada.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductEventRepository, ProductEventRepository>();
        services.AddScoped<IProductEventPublisher, RabbitMqProductEventPublisher>();

        services.AddHostedService<ProductEventConsumerBackgroundService>();

        return services;
    }
}
