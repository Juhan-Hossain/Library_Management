using LibraryManagement.Domain.Services;

namespace LibraryManagement.Application.Features.Loans;

public sealed record BorrowBookCommand(Guid BookId, Guid MemberId) : IRequest<Guid>;

public sealed class BorrowBookCommandValidator : AbstractValidator<BorrowBookCommand>
{
    public BorrowBookCommandValidator()
    {
        RuleFor(c => c.BookId).NotEmpty();
        RuleFor(c => c.MemberId).NotEmpty();
    }
}

public sealed class BorrowBookCommandHandler(
    IBookRepository books,
    IMemberRepository members,
    ILoanRepository loans,
    IUnitOfWork unitOfWork,
    TimeProvider clock) : IRequestHandler<BorrowBookCommand, Guid>
{
    public async Task<Guid> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
    {
        var book = await books.GetRequiredAsync(request.BookId, cancellationToken);
        var member = await members.GetRequiredAsync(request.MemberId, cancellationToken);

        var activeLoanCount = await loans.CountActiveByMemberAsync(member.Id, cancellationToken);
        var alreadyBorrowed = await loans.HasActiveLoanAsync(member.Id, book.Id, cancellationToken);

        var loan = LoanPolicy.Borrow(book, member, activeLoanCount, alreadyBorrowed, clock.GetUtcNow().UtcDateTime);

        loans.Add(loan);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return loan.Id;
    }
}