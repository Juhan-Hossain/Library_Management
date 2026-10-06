namespace LibraryManagement.Application.Features.Libraries;

public sealed record GetLibraryByIdQuery(Guid Id) : IRequest<LibraryDto>;

public sealed class GetLibraryByIdQueryHandler(IReadDbContext db) : IRequestHandler<GetLibraryByIdQuery, LibraryDto>
{
    public async Task<LibraryDto> Handle(GetLibraryByIdQuery request, CancellationToken cancellationToken) =>
        await db.Libraries
            .Where(l => l.Id == request.Id)
            .Select(LibraryProjections.ToDto)
            .FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException(nameof(Library), request.Id);
}