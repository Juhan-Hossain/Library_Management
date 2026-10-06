namespace LibraryManagement.Infrastructure.Persistence.Repositories;

internal sealed class BookRepository(LibraryDbContext dbContext)
    : Repository<Book>(dbContext), IBookRepository
{
    public Task<bool> IsbnExistsAsync(Isbn isbn, Guid? excludingBookId, CancellationToken cancellationToken) =>
        Set.AnyAsync(b => b.Isbn == isbn && b.Id != excludingBookId, cancellationToken);
}