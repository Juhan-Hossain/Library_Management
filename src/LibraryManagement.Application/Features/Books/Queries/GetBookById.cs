namespace LibraryManagement.Application.Features.Books;

public sealed record GetBookByIdQuery(Guid Id) : IRequest<BookDto>;

public sealed class GetBookByIdQueryHandler(IReadDbContext db) : IRequestHandler<GetBookByIdQuery, BookDto>
{
    public async Task<BookDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken) =>
        await db.Books
            .Where(b => b.Id == request.Id)
            .Select(BookProjections.ToDto)
            .FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException(nameof(Book), request.Id);
}