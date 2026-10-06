namespace LibraryManagement.Application.Features.Libraries;

public sealed record GetLibrariesQuery : IRequest<PagedResult<LibraryDto>>, IPagedQuery
{
    public string? Search { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = Paging.DefaultPageSize;
}

public sealed class GetLibrariesQueryValidator : PagedQueryValidator<GetLibrariesQuery>
{
}

public sealed class GetLibrariesQueryHandler(IReadDbContext db)
    : IRequestHandler<GetLibrariesQuery, PagedResult<LibraryDto>>
{
    public Task<PagedResult<LibraryDto>> Handle(GetLibrariesQuery request, CancellationToken cancellationToken)
    {
        var libraries = db.Libraries;

        var search = request.Search?.Trim();
        if (!string.IsNullOrEmpty(search))
            libraries = libraries.Where(l => l.Name.Contains(search) || l.Address.Contains(search));

        return libraries
            .OrderBy(l => l.Name).ThenBy(l => l.Id)
            .ToPagedResultAsync(LibraryProjections.ToDto, request, cancellationToken);
    }
}