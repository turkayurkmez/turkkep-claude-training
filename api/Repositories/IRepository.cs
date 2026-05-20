using Api.Models;

namespace Api.Repositories;

public interface IRepository<T>
{
    IEnumerable<T> GetAll();
    Task<PagedResult<T>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken);
}
