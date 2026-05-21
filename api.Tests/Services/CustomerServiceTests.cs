using Api.Models;
using Api.Repositories;
using Api.Services;

namespace Api.Tests.Services;

public class CustomerServiceTests
{
    private readonly IRepository<Customer> _repository = Substitute.For<IRepository<Customer>>();
    private readonly CustomerService _sut;

    public CustomerServiceTests()
    {
        _sut = new CustomerService(_repository);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsRepositoryResult()
    {
        var query = new PagedQuery(Page: 1, PageSize: 10);
        var expected = new PagedResult<Customer>
        {
            Items = [new Customer { Id = 1, Name = "Test", Email = "test@example.com" }],
            Page = 1,
            PageSize = 10,
            TotalCount = 1
        };
        _repository.GetPagedAsync(query, Arg.Any<CancellationToken>()).Returns(expected);

        var result = await _sut.GetPagedAsync(query, CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetPagedAsync_PassesQueryToRepository()
    {
        var query = new PagedQuery(Page: 2, PageSize: 25);
        _repository.GetPagedAsync(default!, default).ReturnsForAnyArgs(new PagedResult<Customer>());

        await _sut.GetPagedAsync(query, CancellationToken.None);

        await _repository.Received(1).GetPagedAsync(query, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPagedAsync_PassesCancellationTokenToRepository()
    {
        var query = new PagedQuery();
        using var cts = new CancellationTokenSource();
        _repository.GetPagedAsync(default!, default).ReturnsForAnyArgs(new PagedResult<Customer>());

        await _sut.GetPagedAsync(query, cts.Token);

        await _repository.Received(1).GetPagedAsync(Arg.Any<PagedQuery>(), cts.Token);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsRepositoryResult()
    {
        var expected = new Customer { Id = 2, Name = "Ayşe Kaya", Email = "ayse@example.com" };
        _repository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(expected);

        var result = await _sut.GetByIdAsync(2, CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenRepositoryReturnsNull()
    {
        _repository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Customer?)null);

        var result = await _sut.GetByIdAsync(999, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_PassesCancellationTokenToRepository()
    {
        using var cts = new CancellationTokenSource();
        _repository.GetByIdAsync(default, default).ReturnsForAnyArgs((Customer?)null);

        await _sut.GetByIdAsync(1, cts.Token);

        // Arg.Any<CancellationToken>() değil — kesin token eşleşmesi zorunlu
        await _repository.Received(1).GetByIdAsync(1, cts.Token);
    }
}
