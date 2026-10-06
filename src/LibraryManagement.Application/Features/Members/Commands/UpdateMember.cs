namespace LibraryManagement.Application.Features.Members;

public sealed record UpdateMemberCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    Guid LibraryId) : IRequest, IMemberPayload;

public sealed class UpdateMemberCommandValidator : AbstractValidator<UpdateMemberCommand>
{
    public UpdateMemberCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        Include(new MemberPayloadValidator());
    }
}

public sealed class UpdateMemberCommandHandler(
    IMemberRepository members,
    ILibraryRepository libraries,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateMemberCommand>
{
    public async Task Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await members.GetRequiredAsync(request.Id, cancellationToken);
        await libraries.EnsureExistsAsync(request.LibraryId, cancellationToken);

        var email = Email.Create(request.Email);
        if (await members.EmailExistsAsync(email, excludingMemberId: member.Id, cancellationToken))
            throw new ConflictException($"A member with email '{email}' already exists.");

        member.Update(request.FirstName, request.LastName, email, request.Phone, request.LibraryId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}