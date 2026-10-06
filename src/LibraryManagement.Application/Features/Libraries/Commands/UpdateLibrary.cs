namespace LibraryManagement.Application.Features.Libraries;

public sealed record UpdateLibraryCommand(Guid Id, string Name, string Address, string? Phone)
    : IRequest, ILibraryPayload;

public sealed class UpdateLibraryCommandValidator : AbstractValidator<UpdateLibraryCommand>
{
    public UpdateLibraryCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        Include(new LibraryPayloadValidator());
    }
}

public sealed class UpdateLibraryCommandHandler(ILibraryRepository libraries, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateLibraryCommand>
{
    public async Task Handle(UpdateLibraryCommand request, CancellationToken cancellationToken)
    {
        var library = await libraries.GetRequiredAsync(request.Id, cancellationToken);
        library.Update(request.Name, request.Address, request.Phone);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}