namespace SokoHub.Application.Common.Pagination;

public class PagedRequest
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? Search { get; init; }

    public static PagedRequest Create(int page = 1, int pageSize = 20, string? search = null) =>
        new() { Page = page, PageSize = pageSize, Search = search };
}
