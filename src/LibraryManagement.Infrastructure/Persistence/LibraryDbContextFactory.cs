using Microsoft.EntityFrameworkCore.Design;

namespace LibraryManagement.Infrastructure.Persistence;

public sealed class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
    public LibraryDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=LibraryManagementDb;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        return new LibraryDbContext(options);
    }
}