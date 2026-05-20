using Api.Models;
using Api.Repositories;

namespace Api.Services;

public class CustomerService(IRepository<Customer> repository) : ICustomerService
{
    public Task<PagedResult<Customer>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken)
        => repository.GetPagedAsync(query, cancellationToken);
}
