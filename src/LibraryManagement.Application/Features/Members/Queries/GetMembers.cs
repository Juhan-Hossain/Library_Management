namespace LibraryManagement.Application.Features.Members;

public sealed record GetMembersQuery : IRequest<PagedResult<MemberDto>>, IPagedQuery
{
    /// <summary>Matches first or last name.</summary>
    public string? Search { get; init; }
    public Guid? LibraryId { get; init; }
    public bool? IsActive { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = Paging.DefaultPageSize;
}

public sealed class GetMembersQueryValidator : PagedQueryValidator<GetMembersQuery>
{
}

public sealed class GetMembersQueryHandler(IReadDbContext db)
    : IRequestHandler<GetMembersQuery, PagedResult<MemberDto>>
{
    public Task<PagedResult<MemberDto>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
    {
        var members = db.Members;

        var search = request.Search?.Trim();
        if (!string.IsNullOrEmpty(search))
            members = members.Where(m => m.FirstName.Contains(search) || m.LastName.Contains(search));

        if (request.LibraryId is { } libraryId)
            members = members.Where(m => m.LibraryId == libraryId);

        if (request.IsActive is { } isActive)
            members = members.Where(m => m.IsActive == isActive);

        return members
            .OrderBy(m => m.LastName).ThenBy(m => m.FirstName).ThenBy(m => m.Id)
            .ToPagedResultAsync(MemberProjections.ToDto, request, cancellationToken);
    }
}