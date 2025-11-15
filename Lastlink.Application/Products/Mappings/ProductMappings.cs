using Lastlink.Application.Products.DTOs;
using Lastlink.Domain.Entities;

namespace Lastlink.Application.Products.Mappings;

public static class ProductMappings
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto(product.Id, product.Name, product.Category, product.UnitCost, product.CreatedAt);
    }
}
