using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitialiseDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
        var clock = scope.ServiceProvider.GetRequiredService<TimeProvider>();

        await db.Database.MigrateAsync(cancellationToken);
        await SeedAsync(db, clock, cancellationToken);
    }

    private static async Task SeedAsync(LibraryDbContext db, TimeProvider clock, CancellationToken cancellationToken)
    {
        if (await db.Libraries.AnyAsync(cancellationToken))
            return;

        var central = Library.Create("Central Library", "1 Main Street, Springfield", "+1-555-0100");
        var martin = Author.Create("Robert", "Martin", "Software engineer and author of Clean Code.");
        var fowler = Author.Create("Martin", "Fowler", "Author of Refactoring and Patterns of Enterprise Application Architecture.");

        db.Libraries.Add(central);
        db.Authors.AddRange(martin, fowler);
        db.Books.AddRange(
            Book.Create("Clean Code", Isbn.Create("9780132350884"), 2008, "Software Engineering", 3, central.Id, martin.Id),
            Book.Create("Clean Architecture", Isbn.Create("9780134494166"), 2017, "Software Architecture", 2, central.Id, martin.Id),
            Book.Create("Refactoring", Isbn.Create("9780134757599"), 2018, "Software Engineering", 2, central.Id, fowler.Id));
        db.Members.Add(Member.Create("Ada", "Lovelace", Email.Create("ada.lovelace@example.com"), "+1-555-0101",
            central.Id, DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime)));

        await db.SaveChangesAsync(cancellationToken);
    }
}