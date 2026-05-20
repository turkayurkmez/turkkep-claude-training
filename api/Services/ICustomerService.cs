using Api.Models;

namespace Api.Services;

public interface ICustomerService
{
    Task<PagedResult<Customer>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken);
}
