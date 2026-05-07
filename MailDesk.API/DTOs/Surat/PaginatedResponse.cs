namespace MailDesk.API.DTOs.Surat;

public class PaginatedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public PaginationMeta Meta { get; set; } = new();
}

public class PaginationMeta
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalData { get; set; }
    public int Limit { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPrevPage { get; set; }
}