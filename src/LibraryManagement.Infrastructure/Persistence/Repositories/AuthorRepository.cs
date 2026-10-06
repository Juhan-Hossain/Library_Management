namespace LibraryManagement.Infrastructure.Persistence.Repositories;

internal sealed class AuthorRepository(LibraryDbContext dbContext)
    : Repository<Author>(dbContext), IAuthorRepository
{
    public Task<bool> HasBooksAsync(Guid authorId, CancellationToken cancellationToken) =>
        DbContext.Books.AnyAsync(b => b.AuthorId == authorId, cancellationToken);
}