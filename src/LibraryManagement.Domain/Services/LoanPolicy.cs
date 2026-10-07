using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Services;

public static class LoanPolicy
{
    public const int MaxActiveLoansPerMember = 5;
    public static readonly TimeSpan LoanPeriod = TimeSpan.FromDays(14);

    public static Loan Borrow(Book book, Member member, int activeLoanCount, bool alreadyBorrowedThisBook, DateTime utcNow)
    {
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(member);

        if (!member.IsActive)
            throw new BusinessRuleViolationException("Inactive members cannot borrow books.");
        if (member.LibraryId != book.LibraryId)
            throw new BusinessRuleViolationException("Members can only borrow books from their own library.");
        if (activeLoanCount >= MaxActiveLoansPerMember)
            throw new BusinessRuleViolationException(
                $"A member cannot have more than {MaxActiveLoansPerMember} active loans.");
        if (alreadyBorrowedThisBook)
            throw new BusinessRuleViolationException("The member already has an active loan for this book.");

        book.CheckOut();
        member.RecordBorrowing();
        return Loan.Start(book.Id, member.Id, utcNow, LoanPeriod);
    }

    public static void Return(Loan loan, Book book, DateTime utcNow)
    {
        ArgumentNullException.ThrowIfNull(loan);
        ArgumentNullException.ThrowIfNull(book);
        if (loan.BookId != book.Id)
            throw new ArgumentException("The book does not belong to this loan.", nameof(book));

        loan.Close(utcNow);
        book.ReturnCopy();
    }
}