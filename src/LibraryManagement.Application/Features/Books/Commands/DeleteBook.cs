namespace LibraryManagement.Application.Features.Books;

public sealed record DeleteBookCommand(Guid Id) : IRequest;

public sealed class DeleteBookCommandHandler(IBookRepository books, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteBookCommand>
{
    public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await books.GetRequiredAsync(request.Id, cancellationToken);
        if (book.CopiesOnLoan > 0)
            throw new ConflictException("The book has copies on loan and cannot be deleted.");

        books.Remove(book);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}