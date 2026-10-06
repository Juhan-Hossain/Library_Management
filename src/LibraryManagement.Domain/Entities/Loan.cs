using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public sealed class Loan : BaseEntity
{
    private Loan() { }

    public Guid BookId { get; private set; }
    public Guid MemberId { get; private set; }
    public DateTime LoanDateUtc { get; private set; }
    public DateTime DueDateUtc { get; private set; }
    public DateTime? ReturnedDateUtc { get; private set; }
    public LoanStatus Status { get; private set; }

    public Book? Book { get; private set; }
    public Member? Member { get; private set; }

    public bool IsOverdue(DateTime utcNow) => Status == LoanStatus.Active && utcNow > DueDateUtc;

    internal static Loan Start(Guid bookId, Guid memberId, DateTime loanDateUtc, TimeSpan period) => new()
    {
        BookId = bookId,
        MemberId = memberId,
        LoanDateUtc = loanDateUtc,
        DueDateUtc = loanDateUtc + period,
        Status = LoanStatus.Active
    };

    internal void Close(DateTime utcNow)
    {
        if (Status == LoanStatus.Returned)
            throw new BusinessRuleViolationException("This loan has already been returned.");

        ReturnedDateUtc = utcNow;
        Status = LoanStatus.Returned;
    }
}