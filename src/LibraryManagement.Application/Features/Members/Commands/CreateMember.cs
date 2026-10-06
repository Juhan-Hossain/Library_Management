namespace LibraryManagement.Application.Features.Members;

public sealed record CreateMemberCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    Guid LibraryId) : IRequest<Guid>, IMemberPayload;

public sealed class CreateMemberCommandValidator : AbstractValidator<CreateMemberCommand>
{
    public CreateMemberCommandValidator() => Include(new MemberPayloadValidator());
}

public sealed class CreateMemberCommandHandler(
    IMemberRepository members,
    ILibraryRepository libraries,
    IUnitOfWork unitOfWork,
    TimeProvider clock) : IRequestHandler<CreateMemberCommand, Guid>
{
    public async Task<Guid> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        await libraries.EnsureExistsAsync(request.LibraryId, cancellationToken);

        var email = Email.Create(request.Email);
        if (await members.EmailExistsAsync(email, excludingMemberId: null, cancellationToken))
            throw new ConflictException($"A member with email '{email}' already exists.");

        var membershipDate = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);
        var member = Member.Create(request.FirstName, request.LastName, email, request.Phone,
            request.LibraryId, membershipDate);

        members.Add(member);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return member.Id;
    }
}