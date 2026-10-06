namespace LibraryManagement.Application.Features.Members;

public sealed record GetMemberByIdQuery(Guid Id) : IRequest<MemberDto>;

public sealed class GetMemberByIdQueryHandler(IReadDbContext db) : IRequestHandler<GetMemberByIdQuery, MemberDto>
{
    public async Task<MemberDto> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken) =>
        await db.Members
            .Where(m => m.Id == request.Id)
            .Select(MemberProjections.ToDto)
            .FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException(nameof(Member), request.Id);
}