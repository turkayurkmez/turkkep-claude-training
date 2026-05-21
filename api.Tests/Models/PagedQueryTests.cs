using Api.Models;

namespace Api.Tests.Models;

public class PagedQueryTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(1, 1)]
    [InlineData(5, 5)]
    public void Page_ClampsToMinimumOne(int input, int expected)
    {
        var query = new PagedQuery(Page: input, PageSize: 10);

        query.Page.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(1, 1)]
    [InlineData(50, 50)]
    [InlineData(100, 100)]
    [InlineData(101, 100)]
    [InlineData(999, 100)]
    public void PageSize_ClampsToRange(int input, int expected)
    {
        var query = new PagedQuery(Page: 1, PageSize: input);

        query.PageSize.Should().Be(expected);
    }

    [Fact]
    public void TotalPages_CalculatesCorrectly()
    {
        var result = new PagedResult<object>
        {
            TotalCount = 25,
            PageSize = 10
        };

        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public void TotalPages_IsZero_WhenTotalCountIsZero()
    {
        var result = new PagedResult<object>
        {
            TotalCount = 0,
            PageSize = 10
        };

        result.TotalPages.Should().Be(0);
    }
}
