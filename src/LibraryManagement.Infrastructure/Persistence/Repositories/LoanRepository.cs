namespace LibraryManagement.Infrastructure.Persistence.Repositories;

internal sealed class LoanRepository(LibraryDbContext dbContext)
    : Repository<Loan>(dbContext), ILoanRepository
{
    public Task<int> CountActiveByMemberAsync(Guid memberId, CancellationToken cancellationToken) =>
        Set.CountAsync(l => l.MemberId == memberId && l.Status == LoanStatus.Active, cancellationToken);

    public Task<bool> HasActiveLoanAsync(Guid memberId, Guid bookId, CancellationToken cancellationToken) =>
        Set.AnyAsync(l => l.MemberId == memberId && l.BookId == bookId && l.Status == LoanStatus.Active,
            cancellationToken);

    public Task<bool> HasActiveLoansForMemberAsync(Guid memberId, CancellationToken cancellationToken) =>
        Set.AnyAsync(l => l.MemberId == memberId && l.Status == LoanStatus.Active, cancellationToken);
}