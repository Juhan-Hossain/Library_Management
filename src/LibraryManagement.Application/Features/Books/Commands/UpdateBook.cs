namespace LibraryManagement.Application.Features.Books;

public sealed record UpdateBookCommand(
    Guid Id,
    string Title,
    string Isbn,
    int PublishedYear,
    string Genre,
    int TotalCopies,
    Guid LibraryId,
    Guid AuthorId) : IRequest, IBookPayload;

public sealed class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator(TimeProvider clock)
    {
        RuleFor(c => c.Id).NotEmpty();
        Include(new BookPayloadValidator(clock));
    }
}

public sealed class UpdateBookCommandHandler(
    IBookRepository books,
    ILibraryRepository libraries,
    IAuthorRepository authors,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateBookCommand>
{
    public async Task Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await books.GetRequiredAsync(request.Id, cancellationToken);
        await libraries.EnsureExistsAsync(request.LibraryId, cancellationToken);
        await authors.EnsureExistsAsync(request.AuthorId, cancellationToken);

        var isbn = Isbn.Create(request.Isbn);
        if (await books.IsbnExistsAsync(isbn, excludingBookId: book.Id, cancellationToken))
            throw new ConflictException($"A book with ISBN '{isbn}' already exists.");

        book.Update(request.Title, isbn, request.PublishedYear, request.Genre,
            request.TotalCopies, request.LibraryId, request.AuthorId);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}