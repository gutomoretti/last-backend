using Lastlink.Domain.Entities;

namespace Lastlink.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Product>> GetAsync(CancellationToken cancellationToken);
    Task AddAsync(Product product, CancellationToken cancellationToken);
    Task UpdateAsync(Product product, CancellationToken cancellationToken);
    Task DeleteAsync(Product product, CancellationToken cancellationToken);
}
