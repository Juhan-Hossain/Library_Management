using System.Linq.Expressions;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Features.Loans;

public sealed record LoanDto(
    Guid Id,
    Guid BookId,
    string BookTitle,
    Guid MemberId,
    string MemberName,
    DateTime LoanDateUtc,
    DateTime DueDateUtc,
    DateTime? ReturnedDateUtc,
    LoanStatus Status,
    bool IsOverdue);

internal static class LoanProjections
{
    public static Expression<Func<Loan, LoanDto>> ToDto(DateTime utcNow) => l => new LoanDto(
        l.Id,
        l.BookId,
        l.Book!.Title,
        l.MemberId,
        l.Member!.FirstName + " " + l.Member.LastName,
        l.LoanDateUtc,
        l.DueDateUtc,
        l.ReturnedDateUtc,
        l.Status,
        l.Status == LoanStatus.Active && l.DueDateUtc < utcNow);
}