using Lastlink.Application.Common.Exceptions;
using Lastlink.Application.Products.DTOs;
using Lastlink.Application.Products.Mappings;
using Lastlink.Domain.Repositories;
using MediatR;

namespace Lastlink.Application.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException($"Product {request.Id} was not found.");
        }

        return product.ToDto();
    }
}
