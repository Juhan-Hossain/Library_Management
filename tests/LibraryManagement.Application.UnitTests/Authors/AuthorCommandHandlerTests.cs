using LibraryManagement.Application.Features.Authors;

namespace LibraryManagement.Application.UnitTests.Authors;

public sealed class AuthorCommandHandlerTests
{
    private readonly IAuthorRepository _authors = Substitute.For<IAuthorRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Create_adds_author_and_saves()
    {
        var handler = new CreateAuthorCommandHandler(_authors, _unitOfWork);

        var id = await handler.Handle(new CreateAuthorCommand("Robert", "Martin", null), CancellationToken.None);

        _authors.Received(1).Add(Arg.Is<Author>(a => a.Id == id && a.LastName == "Martin"));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_author_with_books_throws_conflict()
    {
        var author = Author.Create("Robert", "Martin", null);
        _authors.GetByIdAsync(author.Id, Arg.Any<CancellationToken>()).Returns(author);
        _authors.HasBooksAsync(author.Id, Arg.Any<CancellationToken>()).Returns(true);
        var handler = new DeleteAuthorCommandHandler(_authors, _unitOfWork);

        var act = () => handler.Handle(new DeleteAuthorCommand(author.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
        _authors.DidNotReceive().Remove(Arg.Any<Author>());
    }
}