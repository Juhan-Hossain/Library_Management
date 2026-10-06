namespace LibraryManagement.Application.Features.Authors;

public sealed record UpdateAuthorCommand(Guid Id, string FirstName, string LastName, string? Biography)
    : IRequest, IAuthorPayload;

public sealed class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
{
    public UpdateAuthorCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        Include(new AuthorPayloadValidator());
    }
}

public sealed class UpdateAuthorCommandHandler(IAuthorRepository authors, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateAuthorCommand>
{
    public async Task Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await authors.GetRequiredAsync(request.Id, cancellationToken);
        author.Update(request.FirstName, request.LastName, request.Biography);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}