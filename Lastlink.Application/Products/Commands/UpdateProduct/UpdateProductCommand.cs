using FluentValidation;
using Lastlink.Application.Common.Exceptions;
using Lastlink.Application.Products.DTOs;
using Lastlink.Application.Products.Mappings;
using Lastlink.Domain.Repositories;
using MediatR;

namespace Lastlink.Application.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(Guid Id, string Name, string Category, decimal UnitCost) : IRequest<ProductDto>;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException($"Product {request.Id} was not found.");
        }

        product.Update(request.Name, request.Category, request.UnitCost);

        await _productRepository.UpdateAsync(product, cancellationToken);

        return product.ToDto();
    }
}

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

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
