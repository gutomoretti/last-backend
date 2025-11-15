using System.Linq;
using FluentValidation;
using Lastlink.Application.Abstractions.Messaging;
using Lastlink.Application.Products.DTOs;
using Lastlink.Application.Products.Mappings;
using Lastlink.Domain.Entities;
using Lastlink.Domain.Events;
using Lastlink.Domain.Repositories;
using MediatR;

namespace Lastlink.Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(string Name, string Category, decimal UnitCost) : IRequest<ProductDto>;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductEventPublisher _eventPublisher;

    public CreateProductCommandHandler(IProductRepository productRepository, IProductEventPublisher eventPublisher)
    {
        _productRepository = productRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(request.Name, request.Category, request.UnitCost);

        await _productRepository.AddAsync(product, cancellationToken);

        var domainEvent = product.DomainEvents.OfType<ProductCreatedDomainEvent>().Single();

        await _eventPublisher.PublishProductCreatedAsync(domainEvent, cancellationToken);

        product.ClearDomainEvents();

        return product.ToDto();
    }
}

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Category)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.UnitCost)
            .GreaterThan(0);
    }
}
