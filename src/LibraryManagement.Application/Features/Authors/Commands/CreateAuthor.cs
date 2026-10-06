namespace LibraryManagement.Application.Features.Authors;

public sealed record CreateAuthorCommand(string FirstName, string LastName, string? Biography)
    : IRequest<Guid>, IAuthorPayload;

public sealed class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
{
    public CreateAuthorCommandValidator() => Include(new AuthorPayloadValidator());
}

public sealed class CreateAuthorCommandHandler(IAuthorRepository authors, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateAuthorCommand, Guid>
{
    public async Task<Guid> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = Author.Create(request.FirstName, request.LastName, request.Biography);
        authors.Add(author);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return author.Id;
    }
}