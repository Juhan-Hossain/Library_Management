namespace LibraryManagement.Application.Features.Books;

public sealed record CreateBookCommand(
    string Title,
    string Isbn,
    int PublishedYear,
    string Genre,
    int TotalCopies,
    Guid LibraryId,
    Guid AuthorId) : IRequest<Guid>, IBookPayload;

public sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator(TimeProvider clock) => Include(new BookPayloadValidator(clock));
}

public sealed class CreateBookCommandHandler(
    IBookRepository books,
    ILibraryRepository libraries,
    IAuthorRepository authors,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateBookCommand, Guid>
{
    public async Task<Guid> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        await libraries.EnsureExistsAsync(request.LibraryId, cancellationToken);
        await authors.EnsureExistsAsync(request.AuthorId, cancellationToken);

        var isbn = Isbn.Create(request.Isbn);
        if (await books.IsbnExistsAsync(isbn, excludingBookId: null, cancellationToken))
            throw new ConflictException($"A book with ISBN '{isbn}' already exists.");

        var book = Book.Create(request.Title, isbn, request.PublishedYear, request.Genre,
            request.TotalCopies, request.LibraryId, request.AuthorId);

        books.Add(book);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return book.Id;
    }
}