using LibraryManagement.Domain.Services;

namespace LibraryManagement.Application.Features.Loans;

public sealed record ReturnBookCommand(Guid LoanId) : IRequest;

public sealed class ReturnBookCommandValidator : AbstractValidator<ReturnBookCommand>
{
    public ReturnBookCommandValidator() => RuleFor(c => c.LoanId).NotEmpty();
}

public sealed class ReturnBookCommandHandler(
    ILoanRepository loans,
    IBookRepository books,
    IUnitOfWork unitOfWork,
    TimeProvider clock) : IRequestHandler<ReturnBookCommand>
{
    public async Task Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        var loan = await loans.GetRequiredAsync(request.LoanId, cancellationToken);
        var book = await books.GetRequiredAsync(loan.BookId, cancellationToken);

        LoanPolicy.Return(loan, book, clock.GetUtcNow().UtcDateTime);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}