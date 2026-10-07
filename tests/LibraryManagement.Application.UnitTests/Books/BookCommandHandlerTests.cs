using LibraryManagement.Application.Features.Books;

namespace LibraryManagement.Application.UnitTests.Books;

public sealed class BookCommandHandlerTests
{
    private readonly IBookRepository _books = Substitute.For<IBookRepository>();
    private readonly ILibraryRepository _libraries = Substitute.For<ILibraryRepository>();
    private readonly IAuthorRepository _authors = Substitute.For<IAuthorRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public BookCommandHandlerTests()
    {
        _libraries.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _authors.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
    }

    private static CreateBookCommand NewCommand() =>
        new("Clean Code", "978-0-13-235088-4", 2008, "Software", 2, Guid.NewGuid(), Guid.NewGuid());

    [Fact]
    public async Task Create_adds_book_with_normalised_isbn_and_saves()
    {
        var handler = new CreateBookCommandHandler(_books, _libraries, _authors, _unitOfWork);

        var id = await handler.Handle(NewCommand(), CancellationToken.None);

        _books.Received(1).Add(Arg.Is<Book>(b =>
            b.Id == id && b.Isbn.Value == "9780132350884" && b.AvailableCopies == 2));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_with_unknown_library_throws_not_found()
    {
        _libraries.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);
        var handler = new CreateBookCommandHandler(_books, _libraries, _authors, _unitOfWork);

        var act = () => handler.Handle(NewCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Library*");
        _books.DidNotReceive().Add(Arg.Any<Book>());
    }

    [Fact]
    public async Task Create_with_duplicate_isbn_throws_conflict()
    {
        _books.IsbnExistsAsync(Arg.Any<Guid>(), Arg.Any<Isbn>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(true);

        var handler = new CreateBookCommandHandler(_books, _libraries, _authors, _unitOfWork);

        var act = () => handler.Handle(NewCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>().WithMessage("*9780132350884*");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_checks_isbn_uniqueness_excluding_itself_and_saves()
    {
        var book = TestData.NewBook(Guid.NewGuid(), copies: 2);
        _books.GetByIdAsync(book.Id, Arg.Any<CancellationToken>()).Returns(book);
        var handler = new UpdateBookCommandHandler(_books, _libraries, _authors, _unitOfWork);

        await handler.Handle(
            new UpdateBookCommand(book.Id, "Clean Code (2nd ed.)", "9780132350884", 2009, "Software", 3, book.LibraryId, book.AuthorId),
            CancellationToken.None);

        await _books.Received(1).IsbnExistsAsync(book.LibraryId, Arg.Any<Isbn>(), book.Id, Arg.Any<CancellationToken>());
        book.Title.Should().Be("Clean Code (2nd ed.)");
        book.TotalCopies.Should().Be(3);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_book_with_copies_on_loan_throws_conflict()
    {
        var book = TestData.NewBook(Guid.NewGuid(), copies: 2);
        book.CheckOut();
        _books.GetByIdAsync(book.Id, Arg.Any<CancellationToken>()).Returns(book);
        var handler = new DeleteBookCommandHandler(_books, _unitOfWork);

        var act = () => handler.Handle(new DeleteBookCommand(book.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
        _books.DidNotReceive().Remove(Arg.Any<Book>());
    }
}