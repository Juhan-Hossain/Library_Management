namespace LibraryManagement.Application.Common.Models;

public interface IPagedQuery
{
    int Page { get; }
    int PageSize { get; }
}

public static class Paging
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;
}