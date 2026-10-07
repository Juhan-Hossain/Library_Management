namespace LibraryManagement.Infrastructure.Persistence.Repositories;

internal sealed class BookRepository(LibraryDbContext dbContext)
    : Repository<Book>(dbContext), IBookRepository
{
    public Task<bool> IsbnExistsAsync(Guid libraryId, Isbn isbn, Guid? excludingBookId, CancellationToken cancellationToken) =>
    Set.AnyAsync(b => b.LibraryId == libraryId && b.Isbn == isbn && b.Id != excludingBookId, cancellationToken);
}