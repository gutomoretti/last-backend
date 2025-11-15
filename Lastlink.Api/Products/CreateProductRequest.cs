namespace Lastlink.Api.Products;

public sealed record CreateProductRequest(string Name, string Category, decimal UnitCost);
