namespace LibraryManagement.Application.Features.Members;

public sealed record SetMemberStatusCommand(Guid Id, bool IsActive) : IRequest;

public sealed class SetMemberStatusCommandHandler(IMemberRepository members, IUnitOfWork unitOfWork)
    : IRequestHandler<SetMemberStatusCommand>
{
    public async Task Handle(SetMemberStatusCommand request, CancellationToken cancellationToken)
    {
        var member = await members.GetRequiredAsync(request.Id, cancellationToken);

        if (request.IsActive)
            member.Activate();
        else
            member.Deactivate();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}