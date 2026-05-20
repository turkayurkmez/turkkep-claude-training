namespace Api.Models;

public record PagedQuery(int Page = 1, int PageSize = 10)
{
    public int Page { get; init; } = Math.Max(1, Page);
    public int PageSize { get; init; } = Math.Clamp(PageSize, 1, 100);
}
