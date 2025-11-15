using FluentValidation;
using Lastlink.Application.Common.Exceptions;
using Lastlink.Domain.Repositories;
using MediatR;

namespace Lastlink.Application.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : IRequest<Unit>;

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException($"Product {request.Id} was not found.");
        }

        await _productRepository.DeleteAsync(product, cancellationToken);

        return Unit.Value;
    }
}

public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
