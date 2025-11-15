namespace Lastlink.Api.Products;

public sealed record UpdateProductRequest(string Name, string Category, decimal UnitCost);
