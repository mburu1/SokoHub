namespace SokoHub.Application.Common.Pagination;

public class PagedResult<T>(
    IReadOnlyList<T> items,
    int totalCount,
    int page,
    int pageSize)
{
    public IReadOnlyList<T> Items { get; } = items;

    public int TotalCount { get; } = totalCount;

    public int Page { get; } = page;

    public int PageSize { get; } = pageSize;

    public int TotalPages => (int)Math.Ceiling(totalCount / (double)pageSize);

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public static PagedResult<T> From(IEnumerable<T> items, int totalCount, int page, int pageSize) =>
        new(items.ToList(), totalCount, page, pageSize);
}
