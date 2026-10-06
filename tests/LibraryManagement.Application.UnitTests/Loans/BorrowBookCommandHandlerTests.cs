using LibraryManagement.Application.Features.Loans;

namespace LibraryManagement.Application.UnitTests.Loans;

public sealed class BorrowBookCommandHandlerTests
{
    private readonly IBookRepository _books = Substitute.For<IBookRepository>();
    private readonly IMemberRepository _members = Substitute.For<IMemberRepository>();
    private readonly ILoanRepository _loans = Substitute.For<ILoanRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly FakeTimeProvider _clock = TestData.Clock();
    private readonly Book _book;
    private readonly Member _member;
    private readonly BorrowBookCommandHandler _handler;

    public BorrowBookCommandHandlerTests()
    {
        var libraryId = Guid.NewGuid();
        _book = TestData.NewBook(libraryId, copies: 1);
        _member = TestData.NewMember(libraryId);

        _books.GetByIdAsync(_book.Id, Arg.Any<CancellationToken>()).Returns(_book);
        _members.GetByIdAsync(_member.Id, Arg.Any<CancellationToken>()).Returns(_member);
        _loans.CountActiveByMemberAsync(_member.Id, Arg.Any<CancellationToken>()).Returns(0);
        _loans.HasActiveLoanAsync(_member.Id, _book.Id, Arg.Any<CancellationToken>()).Returns(false);

        _handler = new BorrowBookCommandHandler(_books, _members, _loans, _unitOfWork, _clock);
    }

    [Fact]
    public async Task Borrow_creates_loan_due_in_14_days_checks_out_copy_and_saves()
    {
        var loanId = await _handler.Handle(new BorrowBookCommand(_book.Id, _member.Id), CancellationToken.None);

        var now = _clock.GetUtcNow().UtcDateTime;
        _loans.Received(1).Add(Arg.Is<Loan>(l =>
            l.Id == loanId &&
            l.BookId == _book.Id &&
            l.MemberId == _member.Id &&
            l.DueDateUtc == now.AddDays(14)));
        _book.AvailableCopies.Should().Be(0);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Member_at_loan_limit_cannot_borrow()
    {
        _loans.CountActiveByMemberAsync(_member.Id, Arg.Any<CancellationToken>()).Returns(LoanPolicy.MaxActiveLoansPerMember);

        var act = () => _handler.Handle(new BorrowBookCommand(_book.Id, _member.Id), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleViolationException>();
        _book.AvailableCopies.Should().Be(1);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Member_already_holding_the_book_cannot_borrow_it_again()
    {
        _loans.HasActiveLoanAsync(_member.Id, _book.Id, Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _handler.Handle(new BorrowBookCommand(_book.Id, _member.Id), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleViolationException>().WithMessage("*already*");
    }

    [Fact]
    public async Task Unknown_book_throws_not_found()
    {
        var act = () => _handler.Handle(new BorrowBookCommand(Guid.NewGuid(), _member.Id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Book*");
    }
}