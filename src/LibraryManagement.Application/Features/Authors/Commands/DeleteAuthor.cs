namespace LibraryManagement.Application.Features.Authors;

public sealed record DeleteAuthorCommand(Guid Id) : IRequest;

public sealed class DeleteAuthorCommandHandler(IAuthorRepository authors, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteAuthorCommand>
{
    public async Task Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await authors.GetRequiredAsync(request.Id, cancellationToken);
        if (await authors.HasBooksAsync(author.Id, cancellationToken))
            throw new ConflictException("The author still has books and cannot be deleted.");

        authors.Remove(author);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}