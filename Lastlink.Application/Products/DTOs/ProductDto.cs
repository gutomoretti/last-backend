namespace Lastlink.Application.Products.DTOs;

public sealed record ProductDto(Guid Id, string Name, string Category, decimal UnitCost, DateTime CreatedAt);
