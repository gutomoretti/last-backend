using Lastlink.Domain.Entities;
using Lastlink.Domain.Repositories;

namespace Lastlink.Infrastructure.Persistence.Repositories;

public sealed class ProductEventRepository : IProductEventRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductEventRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(ProductEvent productEvent, CancellationToken cancellationToken)
    {
        await _dbContext.ProductEvents.AddAsync(productEvent, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
