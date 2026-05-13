namespace TalkHub.Application.Common.Models;

public class PaginatedResponse<T>
{
    public bool Success { get; set; } = true;
    public int StatusCode { get; set; } = 200;
    public string Message { get; set; } = "Lấy danh sách thành công.";
    public PagedData<T> Data { get; set; } = new();
    public List<ApiError>? Errors { get; set; } = null;
}

public class PagedData<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public MetaData MetaData { get; set; } = new();
}

public class MetaData
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage => CurrentPage < TotalPages;
    public bool HasPreviousPage => CurrentPage > 1;
}
