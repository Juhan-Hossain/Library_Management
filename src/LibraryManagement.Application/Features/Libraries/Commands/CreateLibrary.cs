namespace LibraryManagement.Application.Features.Libraries;

public sealed record CreateLibraryCommand(string Name, string Address, string? Phone)
    : IRequest<Guid>, ILibraryPayload;

public sealed class CreateLibraryCommandValidator : AbstractValidator<CreateLibraryCommand>
{
    public CreateLibraryCommandValidator() => Include(new LibraryPayloadValidator());
}

public sealed class CreateLibraryCommandHandler(ILibraryRepository libraries, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateLibraryCommand, Guid>
{
    public async Task<Guid> Handle(CreateLibraryCommand request, CancellationToken cancellationToken)
    {
        var library = Library.Create(request.Name, request.Address, request.Phone);
        libraries.Add(library);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return library.Id;
    }
}