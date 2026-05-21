using Api.Models;

namespace Api.Repositories;

public class CustomerRepository : IRepository<Customer>
{
    private static readonly List<Customer> _customers =
    [
        new Customer { Id = 1, Name = "Ahmet Yılmaz",  Email = "ahmet@example.com"  },
        new Customer { Id = 2, Name = "Ayşe Kaya",     Email = "ayse@example.com"   },
        new Customer { Id = 3, Name = "Mehmet Demir",  Email = "mehmet@example.com" },
    ];

    public IEnumerable<Customer> GetAll() => _customers;

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();
        return _customers.FirstOrDefault(c => c.Id == id);
    }

    public async Task<PagedResult<Customer>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken)
    {
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();
        var items = _customers
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize);

        return new PagedResult<Customer>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = _customers.Count
        };
    }
}
