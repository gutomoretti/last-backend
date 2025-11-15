using Lastlink.Domain.Entities;

namespace Lastlink.Domain.Repositories;

public interface IProductEventRepository
{
    Task AddAsync(ProductEvent productEvent, CancellationToken cancellationToken);
}
