using Microsoft.AspNetCore.Mvc;
using Api.Controllers;
using Api.Models;
using Api.Repositories;
using Api.Services;

namespace Api.Tests.Controllers;

// Mock kullanmaz — gerçek CustomerRepository + CustomerService + CustomersController zinciri
public class CustomersControllerSociableTests
{
    private readonly CustomersController _sut;

    public CustomersControllerSociableTests()
    {
        var repository = new CustomerRepository();
        var service    = new CustomerService(repository);
        _sut = new CustomersController(service);
    }

    [Fact]
    public async Task Get_ReturnsRealCustomers_FromInMemoryStore()
    {
        var result = await _sut.Get(new PagedQuery(Page: 1, PageSize: 10), CancellationToken.None);

        var paged = result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<PagedResult<Customer>>().Subject;

        paged.TotalCount.Should().Be(3);
        paged.Items.Should().SatisfyRespectively(
            first  => { first.Name.Should().Be("Ahmet Yılmaz"); first.Email.Should().Be("ahmet@example.com"); },
            second => { second.Name.Should().Be("Ayşe Kaya");   second.Email.Should().Be("ayse@example.com"); },
            third  => { third.Name.Should().Be("Mehmet Demir"); third.Email.Should().Be("mehmet@example.com"); });
    }

    [Fact]
    public async Task Get_PropagatesCancellation_WhenTokenAlreadyCancelled()
    {
        // Mutant'ta controller CancellationToken.None geçirir → ThrowIfCancellationRequested() tetiklenmez
        // Doğru implementasyonda gerçek token geçirilir → OperationCanceledException fırlar
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = () => _sut.Get(new PagedQuery(), cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task GetById_ReturnsCorrectCustomer_ForEachId()
    {
        var result1 = await _sut.GetById(1, CancellationToken.None);
        result1.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<Customer>()
            .Which.Name.Should().Be("Ahmet Yılmaz");

        var result2 = await _sut.GetById(2, CancellationToken.None);
        result2.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<Customer>()
            .Which.Name.Should().Be("Ayşe Kaya");
    }

    [Fact]
    public async Task GetById_PropagatesCancellation_WhenTokenAlreadyCancelled()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = () => _sut.GetById(1, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task GetById_NonExistentId_ReturnsNotFound()
    {
        var result = await _sut.GetById(999, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData(1, 2, "Ahmet Yılmaz", "Ayşe Kaya")]
    [InlineData(2, 2, "Mehmet Demir")]
    public async Task Get_PagedCorrectly_ReturnsExpectedSlice(int page, int pageSize, params string[] expectedNames)
    {
        var result = await _sut.Get(new PagedQuery(Page: page, PageSize: pageSize), CancellationToken.None);

        var paged = result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<PagedResult<Customer>>().Subject;

        paged.Items.Select(c => c.Name).Should().Equal(expectedNames);
        paged.TotalCount.Should().Be(3);
        paged.Page.Should().Be(page);
    }
}
