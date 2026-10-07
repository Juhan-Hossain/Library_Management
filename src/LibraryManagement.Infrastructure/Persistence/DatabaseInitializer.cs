using LibraryManagement.Domain.Services;
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

        var now = clock.GetUtcNow().UtcDateTime;
        var today = DateOnly.FromDateTime(now);

        var central = Library.Create("Central Library", "1 Main Street, Springfield", "+1-555-0100");
        var westside = Library.Create("Westside Branch", "42 Ocean Avenue, Springfield", "+1-555-0200");
        var university = Library.Create("University Library", "100 College Road, Springfield", "+1-555-0300");

        var martin = Author.Create("Robert C.", "Martin", "Software engineer; author of Clean Code and Clean Architecture.");
        var fowler = Author.Create("Martin", "Fowler", "Chief Scientist at Thoughtworks; author of Refactoring.");
        var gamma = Author.Create("Erich", "Gamma", "Co-author of Design Patterns (Gang of Four).");
        var evans = Author.Create("Eric", "Evans", "Originator of Domain-Driven Design.");
        var thomas = Author.Create("David", "Thomas", "Co-author of The Pragmatic Programmer.");
        var beck = Author.Create("Kent", "Beck", "Creator of Extreme Programming and Test-Driven Development.");
        var vernon = Author.Create("Vaughn", "Vernon", "Author of Implementing Domain-Driven Design.");
        var feathers = Author.Create("Michael", "Feathers", "Author of Working Effectively with Legacy Code.");
        var mcconnell = Author.Create("Steve", "McConnell", "Author of Code Complete.");

        Book NewBook(string title, string isbn, int year, string genre, int copies, Library library, Author author) =>
            Book.Create(title, Isbn.Create(isbn), year, genre, copies, library.Id, author.Id);

        var cleanCode = NewBook("Clean Code", "9780132350884", 2008, "Software Engineering", 3, central, martin);
        var cleanArchitecture = NewBook("Clean Architecture", "9780134494166", 2017, "Software Architecture", 2, central, martin);
        var refactoring = NewBook("Refactoring", "9780134757599", 2018, "Software Engineering", 2, central, fowler);
        var designPatterns = NewBook("Design Patterns", "9780201633610", 1994, "Software Design", 2, central, gamma);
        var dddCentral = NewBook("Domain-Driven Design", "9780321125217", 2003, "Software Design", 1, central, evans);
        var pragmatic = NewBook("The Pragmatic Programmer", "9780135957059", 2019, "Software Engineering", 2, central, thomas);

        var cleanCodeWestside = NewBook("Clean Code", "9780132350884", 2008, "Software Engineering", 1, westside, martin);
        var poeaa = NewBook("Patterns of Enterprise Application Architecture", "9780321127426", 2002, "Software Architecture", 1, westside, fowler);
        var tdd = NewBook("Test Driven Development: By Example", "9780321146533", 2002, "Testing", 2, westside, beck);
        var legacyCode = NewBook("Working Effectively with Legacy Code", "9780131177055", 2004, "Software Engineering", 1, westside, feathers);

        var codeComplete = NewBook("Code Complete", "9780735619678", 2004, "Software Engineering", 3, university, mcconnell);
        var iddd = NewBook("Implementing Domain-Driven Design", "9780321834577", 2013, "Software Design", 2, university, vernon);
        var xp = NewBook("Extreme Programming Explained", "9780321278654", 2004, "Agile", 1, university, beck);
        var dddUniversity = NewBook("Domain-Driven Design", "9780321125217", 2003, "Software Design", 2, university, evans);

        Member NewMember(string first, string last, string email, string? phone, Library library, int memberForDays) =>
            Member.Create(first, last, Email.Create(email), phone, library.Id, today.AddDays(-memberForDays));

        var ada = NewMember("Ada", "Lovelace", "ada.lovelace@example.com", "+1-555-0101", central, 400);
        var alan = NewMember("Alan", "Turing", "alan.turing@example.com", "+1-555-0102", central, 250);
        var grace = NewMember("Grace", "Hopper", "grace.hopper@example.com", null, central, 120);
        var charles = NewMember("Charles", "Babbage", "charles.babbage@example.com", null, central, 900);
        charles.Deactivate();

        var linus = NewMember("Linus", "Torvalds", "linus.torvalds@example.com", "+1-555-0201", westside, 60);
        var margaret = NewMember("Margaret", "Hamilton", "margaret.hamilton@example.com", null, westside, 30);

        var donald = NewMember("Donald", "Knuth", "donald.knuth@example.com", "+1-555-0301", university, 365);
        var barbara = NewMember("Barbara", "Liskov", "barbara.liskov@example.com", null, university, 200);

        var loans = new List<Loan>();

        Loan Borrow(Book book, Member member, int daysAgo)
        {
            var activeCount = loans.Count(l => l.MemberId == member.Id && l.Status == LoanStatus.Active);
            var loan = LoanPolicy.Borrow(book, member, activeCount, alreadyBorrowedThisBook: false, now.AddDays(-daysAgo));
            loans.Add(loan);
            return loan;
        }

        Borrow(cleanCode, ada, daysAgo: 3);
        Borrow(refactoring, ada, daysAgo: 20);
        Borrow(dddCentral, alan, daysAgo: 5);
        LoanPolicy.Return(Borrow(designPatterns, grace, daysAgo: 30), designPatterns, now.AddDays(-18));
        Borrow(tdd, linus, daysAgo: 2);
        Borrow(legacyCode, margaret, daysAgo: 16);
        Borrow(codeComplete, donald, daysAgo: 1);
        LoanPolicy.Return(Borrow(iddd, barbara, daysAgo: 40), iddd, now.AddDays(-30));

        db.Libraries.AddRange(central, westside, university);
        db.Authors.AddRange(martin, fowler, gamma, evans, thomas, beck, vernon, feathers, mcconnell);
        db.Books.AddRange(
            cleanCode, cleanArchitecture, refactoring, designPatterns, dddCentral, pragmatic,
            cleanCodeWestside, poeaa, tdd, legacyCode,
            codeComplete, iddd, xp, dddUniversity);
        db.Members.AddRange(ada, alan, grace, charles, linus, margaret, donald, barbara);
        db.Loans.AddRange(loans);

        await db.SaveChangesAsync(cancellationToken);
    }
}