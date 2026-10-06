using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Features.Loans;

public sealed record GetLoansQuery : IRequest<PagedResult<LoanDto>>, IPagedQuery
{
    public Guid? MemberId { get; init; }
    public Guid? BookId { get; init; }
    public LoanStatus? Status { get; init; }
    /// <summary>true = only overdue loans; false = only loans that are not overdue.</summary>
    public bool? Overdue { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = Paging.DefaultPageSize;
}

public sealed class GetLoansQueryValidator : PagedQueryValidator<GetLoansQuery>
{
    public GetLoansQueryValidator()
    {
        RuleFor(q => q.Status).IsInEnum().When(q => q.Status.HasValue);
    }
}

public sealed class GetLoansQueryHandler(IReadDbContext db, TimeProvider clock)
    : IRequestHandler<GetLoansQuery, PagedResult<LoanDto>>
{
    public Task<PagedResult<LoanDto>> Handle(GetLoansQuery request, CancellationToken cancellationToken)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        var loans = db.Loans;

        if (request.MemberId is { } memberId)
            loans = loans.Where(l => l.MemberId == memberId);

        if (request.BookId is { } bookId)
            loans = loans.Where(l => l.BookId == bookId);

        if (request.Status is { } status)
            loans = loans.Where(l => l.Status == status);

        if (request.Overdue is { } overdue)
            loans = overdue
                ? loans.Where(l => l.Status == LoanStatus.Active && l.DueDateUtc < now)
                : loans.Where(l => l.Status != LoanStatus.Active || l.DueDateUtc >= now);

        return loans
            .OrderByDescending(l => l.LoanDateUtc).ThenBy(l => l.Id)
            .ToPagedResultAsync(LoanProjections.ToDto(now), request, cancellationToken);
    }
}