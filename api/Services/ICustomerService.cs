using Api.Models;

namespace Api.Services;

public interface ICustomerService
{
    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<PagedResult<Customer>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken);
}
