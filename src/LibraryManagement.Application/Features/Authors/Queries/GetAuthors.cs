namespace LibraryManagement.Application.Features.Authors;

public sealed record GetAuthorsQuery : IRequest<PagedResult<AuthorDto>>, IPagedQuery
{
    /// <summary>Matches first or last name.</summary>
    public string? Search { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = Paging.DefaultPageSize;
}

public sealed class GetAuthorsQueryValidator : PagedQueryValidator<GetAuthorsQuery>
{
}

public sealed class GetAuthorsQueryHandler(IReadDbContext db)
    : IRequestHandler<GetAuthorsQuery, PagedResult<AuthorDto>>
{
    public Task<PagedResult<AuthorDto>> Handle(GetAuthorsQuery request, CancellationToken cancellationToken)
    {
        var authors = db.Authors;

        var search = request.Search?.Trim();
        if (!string.IsNullOrEmpty(search))
            authors = authors.Where(a => a.FirstName.Contains(search) || a.LastName.Contains(search));

        return authors
            .OrderBy(a => a.LastName).ThenBy(a => a.FirstName).ThenBy(a => a.Id)
            .ToPagedResultAsync(AuthorProjections.ToDto, request, cancellationToken);
    }
}