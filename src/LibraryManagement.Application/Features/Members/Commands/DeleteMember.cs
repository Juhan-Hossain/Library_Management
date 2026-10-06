namespace LibraryManagement.Application.Features.Members;

public sealed record DeleteMemberCommand(Guid Id) : IRequest;

public sealed class DeleteMemberCommandHandler(
    IMemberRepository members,
    ILoanRepository loans,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteMemberCommand>
{
    public async Task Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await members.GetRequiredAsync(request.Id, cancellationToken);
        if (await loans.HasActiveLoansForMemberAsync(member.Id, cancellationToken))
            throw new ConflictException("The member has active loans and cannot be deleted.");

        members.Remove(member);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}