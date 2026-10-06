using System.Linq.Expressions;

namespace LibraryManagement.Application.Common.Models;

public static class QueryableExtensions
{
    public static async Task<PagedResult<TResult>> ToPagedResultAsync<TSource, TResult>(
        this IQueryable<TSource> source,
        Expression<Func<TSource, TResult>> projection,
        IPagedQuery paging,
        CancellationToken cancellationToken)
    {
        var totalCount = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .Select(projection)
            .ToListAsync(cancellationToken);

        return new PagedResult<TResult>(items, paging.Page, paging.PageSize, totalCount);
    }
}