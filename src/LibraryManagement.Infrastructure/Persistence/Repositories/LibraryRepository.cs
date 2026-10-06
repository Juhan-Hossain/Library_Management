namespace LibraryManagement.Infrastructure.Persistence.Repositories;

internal sealed class LibraryRepository(LibraryDbContext dbContext)
    : Repository<Library>(dbContext), ILibraryRepository
{
    public async Task<bool> HasBooksOrMembersAsync(Guid libraryId, CancellationToken cancellationToken) =>
        await DbContext.Books.AnyAsync(b => b.LibraryId == libraryId, cancellationToken) ||
        await DbContext.Members.AnyAsync(m => m.LibraryId == libraryId, cancellationToken);
}