namespace LibraryManagement.Application.Features.Authors;

public sealed record GetAuthorByIdQuery(Guid Id) : IRequest<AuthorDto>;

public sealed class GetAuthorByIdQueryHandler(IReadDbContext db) : IRequestHandler<GetAuthorByIdQuery, AuthorDto>
{
    public async Task<AuthorDto> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken) =>
        await db.Authors
            .Where(a => a.Id == request.Id)
            .Select(AuthorProjections.ToDto)
            .FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException(nameof(Author), request.Id);
}