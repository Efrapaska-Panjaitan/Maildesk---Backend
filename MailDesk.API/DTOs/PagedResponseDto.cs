namespace Maildesk.Api.Dtos;

public class PagedResponseDto<T>
{
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    public PaginationMeta Meta { get; set; } = new();
}

public class PaginationMeta
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalData { get; set; }
    public int TotalPage { get; set; }
    public string SortBy { get; set; } = "";
    public string SortDirection { get; set; } = "";
}