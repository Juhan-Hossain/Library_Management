using LibraryManagement.Application.Features.Loans;

namespace LibraryManagement.Application.UnitTests.Loans;

public sealed class ReturnBookCommandHandlerTests
{
    private readonly ILoanRepository _loans = Substitute.For<ILoanRepository>();
    private readonly IBookRepository _books = Substitute.For<IBookRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly FakeTimeProvider _clock = TestData.Clock();

    [Fact]
    public async Task Return_closes_loan_restores_copy_and_saves()
    {
        var libraryId = Guid.NewGuid();
        var book = TestData.NewBook(libraryId, copies: 1);
        var loan = LoanPolicy.Borrow(book, TestData.NewMember(libraryId), 0, false, _clock.GetUtcNow().UtcDateTime);
        _loans.GetByIdAsync(loan.Id, Arg.Any<CancellationToken>()).Returns(loan);
        _books.GetByIdAsync(book.Id, Arg.Any<CancellationToken>()).Returns(book);
        _clock.Advance(TimeSpan.FromDays(3));
        var handler = new ReturnBookCommandHandler(_loans, _books, _unitOfWork, _clock);

        await handler.Handle(new ReturnBookCommand(loan.Id), CancellationToken.None);

        loan.Status.Should().Be(LoanStatus.Returned);
        loan.ReturnedDateUtc.Should().Be(_clock.GetUtcNow().UtcDateTime);
        book.AvailableCopies.Should().Be(1);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Unknown_loan_throws_not_found()
    {
        var handler = new ReturnBookCommandHandler(_loans, _books, _unitOfWork, _clock);

        var act = () => handler.Handle(new ReturnBookCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Loan*");
    }
}