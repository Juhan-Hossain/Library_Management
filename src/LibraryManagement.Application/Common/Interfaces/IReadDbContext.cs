namespace LibraryManagement.Application.Common.Interfaces;

public interface IReadDbContext
{
    IQueryable<Library> Libraries { get; }
    IQueryable<Author> Authors { get; }
    IQueryable<Book> Books { get; }
    IQueryable<Member> Members { get; }
    IQueryable<Loan> Loans { get; }
}