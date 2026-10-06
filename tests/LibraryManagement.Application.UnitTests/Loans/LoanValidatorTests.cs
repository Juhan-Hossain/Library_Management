using FluentValidation.TestHelper;
using LibraryManagement.Application.Features.Loans;

namespace LibraryManagement.Application.UnitTests.Loans;

public sealed class LoanValidatorTests
{
    [Fact]
    public void Borrow_requires_book_and_member_ids()
    {
        var result = new BorrowBookCommandValidator().TestValidate(new BorrowBookCommand(Guid.Empty, Guid.Empty));

        result.ShouldHaveValidationErrorFor(c => c.BookId);
        result.ShouldHaveValidationErrorFor(c => c.MemberId);
    }

    [Fact]
    public void Get_loans_rejects_undefined_status()
    {
        new GetLoansQueryValidator()
            .TestValidate(new GetLoansQuery { Status = (LoanStatus)99 })
            .ShouldHaveValidationErrorFor(q => q.Status);
    }
}