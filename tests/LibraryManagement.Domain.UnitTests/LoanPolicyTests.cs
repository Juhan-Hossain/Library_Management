namespace LibraryManagement.Domain.UnitTests;

public sealed class LoanPolicyTests
{
    private static readonly DateTime Now = DomainFakes.Now;

    [Fact]
    public void Borrow_creates_active_loan_due_in_14_days_and_checks_out_a_copy()
    {
        var book = DomainFakes.NewBook(copies: 2);
        var member = DomainFakes.NewMember();

        var loan = LoanPolicy.Borrow(book, member, activeLoanCount: 0, alreadyBorrowedThisBook: false, Now);

        loan.BookId.Should().Be(book.Id);
        loan.MemberId.Should().Be(member.Id);
        loan.Status.Should().Be(LoanStatus.Active);
        loan.LoanDateUtc.Should().Be(Now);
        loan.DueDateUtc.Should().Be(Now.AddDays(14));
        loan.ReturnedDateUtc.Should().BeNull();
        book.AvailableCopies.Should().Be(1);
    }

    [Fact]
    public void Inactive_member_cannot_borrow()
    {
        var book = DomainFakes.NewBook();
        var member = DomainFakes.NewMember();
        member.Deactivate();

        var act = () => LoanPolicy.Borrow(book, member, 0, false, Now);

        act.Should().Throw<BusinessRuleViolationException>().WithMessage("*Inactive*");
        book.AvailableCopies.Should().Be(2);
    }

    [Fact]
    public void Member_cannot_borrow_from_another_library()
    {
        var book = DomainFakes.NewBook(libraryId: Guid.NewGuid());
        var member = DomainFakes.NewMember();

        var act = () => LoanPolicy.Borrow(book, member, 0, false, Now);

        act.Should().Throw<BusinessRuleViolationException>().WithMessage("*own library*");
    }

    [Fact]
    public void Member_at_loan_limit_cannot_borrow()
    {
        var act = () => LoanPolicy.Borrow(DomainFakes.NewBook(), DomainFakes.NewMember(),
            LoanPolicy.MaxActiveLoansPerMember, false, Now);

        act.Should().Throw<BusinessRuleViolationException>().WithMessage("*more than 5*");
    }

    [Fact]
    public void Member_cannot_borrow_same_book_twice()
    {
        var act = () => LoanPolicy.Borrow(DomainFakes.NewBook(), DomainFakes.NewMember(), 1, true, Now);

        act.Should().Throw<BusinessRuleViolationException>().WithMessage("*already*");
    }

    [Fact]
    public void Borrow_with_no_copies_available_throws()
    {
        var book = DomainFakes.NewBook(copies: 1);
        LoanPolicy.Borrow(book, DomainFakes.NewMember(), 0, false, Now);

        var act = () => LoanPolicy.Borrow(book, DomainFakes.NewMember(), 0, false, Now);

        act.Should().Throw<BusinessRuleViolationException>().WithMessage("*No copies*");
    }

    [Fact]
    public void Return_closes_loan_and_restores_copy()
    {
        var book = DomainFakes.NewBook(copies: 1);
        var loan = LoanPolicy.Borrow(book, DomainFakes.NewMember(), 0, false, Now);

        LoanPolicy.Return(loan, book, Now.AddDays(3));

        loan.Status.Should().Be(LoanStatus.Returned);
        loan.ReturnedDateUtc.Should().Be(Now.AddDays(3));
        book.AvailableCopies.Should().Be(1);
    }

    [Fact]
    public void Returning_twice_throws_and_does_not_add_a_copy()
    {
        var book = DomainFakes.NewBook(copies: 2);
        var loan = LoanPolicy.Borrow(book, DomainFakes.NewMember(), 0, false, Now);
        LoanPolicy.Return(loan, book, Now);

        var act = () => LoanPolicy.Return(loan, book, Now);

        act.Should().Throw<BusinessRuleViolationException>().WithMessage("*already been returned*");
        book.AvailableCopies.Should().Be(2);
    }

    [Fact]
    public void Return_with_wrong_book_throws()
    {
        var loan = LoanPolicy.Borrow(DomainFakes.NewBook(), DomainFakes.NewMember(), 0, false, Now);

        var act = () => LoanPolicy.Return(loan, DomainFakes.NewBook(), Now);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void IsOverdue_only_for_active_loans_past_due_date()
    {
        var book = DomainFakes.NewBook();
        var loan = LoanPolicy.Borrow(book, DomainFakes.NewMember(), 0, false, Now);

        loan.IsOverdue(Now.AddDays(14)).Should().BeFalse();
        loan.IsOverdue(Now.AddDays(15)).Should().BeTrue();

        LoanPolicy.Return(loan, book, Now.AddDays(20));
        loan.IsOverdue(Now.AddDays(30)).Should().BeFalse();
    }

    [Fact]
    public void Borrow_changes_member_version_to_guard_the_loan_limit()
    {
        var member = DomainFakes.NewMember();
        var version = member.Version;

        LoanPolicy.Borrow(DomainFakes.NewBook(), member, 0, false, Now);

        member.Version.Should().NotBe(version);
    }
}