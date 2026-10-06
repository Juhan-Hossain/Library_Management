namespace LibraryManagement.Application.Features.Loans;

public sealed record GetLoanByIdQuery(Guid Id) : IRequest<LoanDto>;

public sealed class GetLoanByIdQueryHandler(IReadDbContext db, TimeProvider clock)
    : IRequestHandler<GetLoanByIdQuery, LoanDto>
{
    public async Task<LoanDto> Handle(GetLoanByIdQuery request, CancellationToken cancellationToken) =>
        await db.Loans
            .Where(l => l.Id == request.Id)
            .Select(LoanProjections.ToDto(clock.GetUtcNow().UtcDateTime))
            .FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException(nameof(Loan), request.Id);
}