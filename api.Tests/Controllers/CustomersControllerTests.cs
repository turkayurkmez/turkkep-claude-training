using Microsoft.AspNetCore.Mvc;
using Api.Controllers;
using Api.Models;
using Api.Services;

namespace Api.Tests.Controllers;

public class CustomersControllerTests
{
    private readonly ICustomerService _customerService = Substitute.For<ICustomerService>();
    private readonly CustomersController _sut;

    public CustomersControllerTests()
    {
        _sut = new CustomersController(_customerService);
    }

    [Fact]
    public async Task Get_ReturnsOkWithPagedResult()
    {
        var query = new PagedQuery(Page: 1, PageSize: 10);
        var pagedResult = new PagedResult<Customer>
        {
            Items = [new Customer { Id = 1, Name = "Alice", Email = "alice@example.com" }],
            Page = 1,
            PageSize = 10,
            TotalCount = 1
        };
        _customerService.GetPagedAsync(query, Arg.Any<CancellationToken>()).Returns(pagedResult);

        var actionResult = await _sut.Get(query, CancellationToken.None);

        actionResult.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(pagedResult);
    }

    [Fact]
    public async Task Get_PassesQueryToService()
    {
        var query = new PagedQuery(Page: 3, PageSize: 5);
        _customerService.GetPagedAsync(default!, default).ReturnsForAnyArgs(new PagedResult<Customer>());

        await _sut.Get(query, CancellationToken.None);

        await _customerService.Received(1).GetPagedAsync(query, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Get_ReturnsEmptyItems_WhenNoCustomers()
    {
        var query = new PagedQuery();
        _customerService.GetPagedAsync(default!, default).ReturnsForAnyArgs(new PagedResult<Customer>
        {
            Items = [],
            TotalCount = 0
        });

        var actionResult = await _sut.Get(query, CancellationToken.None);

        var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        var result = okResult.Value.Should().BeOfType<PagedResult<Customer>>().Subject;
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
