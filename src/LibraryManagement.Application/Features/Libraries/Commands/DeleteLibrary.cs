namespace LibraryManagement.Application.Features.Libraries;

public sealed record DeleteLibraryCommand(Guid Id) : IRequest;

public sealed class DeleteLibraryCommandHandler(ILibraryRepository libraries, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteLibraryCommand>
{
    public async Task Handle(DeleteLibraryCommand request, CancellationToken cancellationToken)
    {
        var library = await libraries.GetRequiredAsync(request.Id, cancellationToken);
        if (await libraries.HasBooksOrMembersAsync(library.Id, cancellationToken))
            throw new ConflictException("The library still has books or members and cannot be deleted.");

        libraries.Remove(library);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}