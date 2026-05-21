using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Api.Models;

namespace Api.Tests.Integration;

public class CustomersEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_Returns200Ok()
    {
        var response = await _client.GetAsync("/customers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_ContentTypeIsJson()
    {
        var response = await _client.GetAsync("/customers");

        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task Get_BodyIsValidJson()
    {
        var response = await _client.GetAsync("/customers");
        var body = await response.Content.ReadAsStringAsync();

        // JsonDocument.Parse geçersiz JSON'da JsonException fırlatır
        var act = () => JsonDocument.Parse(body);
        act.Should().NotThrow(because: "response body geçerli JSON olmalı; alınan: {0}", body);
    }

    [Fact]
    public async Task Get_ResponseShapeHasExpectedProperties()
    {
        var response = await _client.GetAsync("/customers");
        var body = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        root.TryGetProperty("items",      out _).Should().BeTrue(because: "'items' alanı eksik");
        root.TryGetProperty("page",       out _).Should().BeTrue(because: "'page' alanı eksik");
        root.TryGetProperty("pageSize",   out _).Should().BeTrue(because: "'pageSize' alanı eksik");
        root.TryGetProperty("totalCount", out _).Should().BeTrue(because: "'totalCount' alanı eksik");
        root.TryGetProperty("totalPages", out _).Should().BeTrue(because: "'totalPages' alanı eksik");
    }

    [Fact]
    public async Task Get_DefaultQuery_ReturnsAllThreeSeededCustomers()
    {
        var result = await _client.GetFromJsonAsync<PagedResult<Customer>>(
            "/customers",
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        result.Should().NotBeNull();
        result!.TotalCount.Should().Be(3);
        result.Items.Should().HaveCount(3)
            .And.Contain(c => c.Name == "Ahmet Yılmaz")
            .And.Contain(c => c.Name == "Ayşe Kaya")
            .And.Contain(c => c.Name == "Mehmet Demir");
    }

    // --- GET /customers/{id} testleri ---

    [Theory]
    [InlineData(1, "Ahmet Yılmaz")]
    [InlineData(2, "Ayşe Kaya")]
    [InlineData(3, "Mehmet Demir")]
    public async Task GetById_ExistingId_Returns200WithMatchingCustomer(
        int id, string expectedName)
    {
        var customer = await _client.GetFromJsonAsync<Customer>(
            $"/customers/{id}",
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        customer!.Id.Should().Be(id);
        customer.Name.Should().Be(expectedName);
    }

    [Fact]
    public async Task GetById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync("/customers/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public static TheoryData<int, int, string[]> PaginationCases => new()
    {
        { 1, 2,  ["Ahmet Yılmaz", "Ayşe Kaya"] },
        { 2, 2,  ["Mehmet Demir"] },
        { 1, 10, ["Ahmet Yılmaz", "Ayşe Kaya", "Mehmet Demir"] },
    };

    [Theory]
    [MemberData(nameof(PaginationCases))]
    public async Task Get_WithPaginationQuery_ReturnsCorrectSlice(
        int page, int pageSize, string[] expectedNames)
    {
        var result = await _client.GetFromJsonAsync<PagedResult<Customer>>(
            $"/customers?page={page}&pageSize={pageSize}",
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        result!.Items.Select(c => c.Name).Should().Equal(expectedNames);
        result.Page.Should().Be(page);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(3);
    }
}
