using System.Linq;
using Lastlink.Application.Products.DTOs;
using Lastlink.Application.Products.Mappings;
using Lastlink.Domain.Repositories;
using MediatR;

namespace Lastlink.Application.Products.Queries.GetProducts;

public sealed record GetProductsQuery : IRequest<IReadOnlyList<ProductDto>>;

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAsync(cancellationToken);

        return products
            .Select(product => product.ToDto())
            .ToList();
    }
}
