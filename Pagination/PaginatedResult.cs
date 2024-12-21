namespace SimpleArchitecture.Pagination;

public record PaginatedResult<T>(int Count, IReadOnlyList<T> Results, int PageIndex, int PageCount)
{
    public bool HasNextPage => PageIndex * PageCount < Count;

    public bool HasPreviousPage => PageIndex > 1;
    
    public int PagesCount => Count == 0 || Results.Count == 0 ? 0 : Count / PageCount;
}