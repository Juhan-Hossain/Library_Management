using LibraryManagement.Application.Features.Libraries;

namespace LibraryManagement.Application.UnitTests.Libraries;

public sealed class LibraryCommandHandlerTests
{
    private readonly ILibraryRepository _libraries = Substitute.For<ILibraryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Create_adds_library_and_saves()
    {
        var handler = new CreateLibraryCommandHandler(_libraries, _unitOfWork);

        var id = await handler.Handle(new CreateLibraryCommand("Central", "1 Main St", null), CancellationToken.None);

        _libraries.Received(1).Add(Arg.Is<Library>(l => l.Id == id && l.Name == "Central"));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_unknown_library_throws_not_found()
    {
        _libraries.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Library?)null);
        var handler = new UpdateLibraryCommandHandler(_libraries, _unitOfWork);

        var act = () => handler.Handle(new UpdateLibraryCommand(Guid.NewGuid(), "X", "Y", null), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_changes_values_and_saves()
    {
        var library = Library.Create("Central", "1 Main St", null);
        _libraries.GetByIdAsync(library.Id, Arg.Any<CancellationToken>()).Returns(library);
        var handler = new UpdateLibraryCommandHandler(_libraries, _unitOfWork);

        await handler.Handle(new UpdateLibraryCommand(library.Id, "Renamed", "2 High St", "555-0100"), CancellationToken.None);

        library.Name.Should().Be("Renamed");
        library.Phone.Should().Be("555-0100");
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_library_with_books_or_members_throws_conflict()
    {
        var library = Library.Create("Central", "1 Main St", null);
        _libraries.GetByIdAsync(library.Id, Arg.Any<CancellationToken>()).Returns(library);
        _libraries.HasBooksOrMembersAsync(library.Id, Arg.Any<CancellationToken>()).Returns(true);
        var handler = new DeleteLibraryCommandHandler(_libraries, _unitOfWork);

        var act = () => handler.Handle(new DeleteLibraryCommand(library.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
        _libraries.DidNotReceive().Remove(Arg.Any<Library>());
    }

    [Fact]
    public async Task Delete_empty_library_removes_and_saves()
    {
        var library = Library.Create("Central", "1 Main St", null);
        _libraries.GetByIdAsync(library.Id, Arg.Any<CancellationToken>()).Returns(library);
        var handler = new DeleteLibraryCommandHandler(_libraries, _unitOfWork);

        await handler.Handle(new DeleteLibraryCommand(library.Id), CancellationToken.None);

        _libraries.Received(1).Remove(library);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}