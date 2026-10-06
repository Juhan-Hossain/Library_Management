using LibraryManagement.Application.Common.Exceptions;
using Microsoft.Data.SqlClient;

namespace LibraryManagement.Infrastructure.Persistence;

public sealed class LibraryDbContext(DbContextOptions<LibraryDbContext> options)
    : DbContext(options), IReadDbContext, IUnitOfWork
{
    public DbSet<Library> Libraries => Set<Library>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Loan> Loans => Set<Loan>();

    // Read side (CQRS queries): never tracked.
    IQueryable<Library> IReadDbContext.Libraries => Libraries.AsNoTracking();
    IQueryable<Author> IReadDbContext.Authors => Authors.AsNoTracking();
    IQueryable<Book> IReadDbContext.Books => Books.AsNoTracking();
    IQueryable<Member> IReadDbContext.Members => Members.AsNoTracking();
    IQueryable<Loan> IReadDbContext.Loans => Loans.AsNoTracking();

    async Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConflictException("The resource was modified by another request. Reload it and try again.", ex);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new ConflictException("A record with the same unique value already exists.", ex);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);

    private static bool IsUniqueConstraintViolation(DbUpdateException ex) =>
        ex.InnerException is SqlException { Number: 2601 or 2627 };
}