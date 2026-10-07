namespace LibraryManagement.Application.Common.Interfaces;

public interface ILibraryRepository : IRepository<Library>
{
    Task<bool> HasBooksOrMembersAsync(Guid libraryId, CancellationToken cancellationToken);
}

public interface IAuthorRepository : IRepository<Author>
{
    Task<bool> HasBooksAsync(Guid authorId, CancellationToken cancellationToken);
}

public interface IBookRepository : IRepository<Book>
{
    Task<bool> IsbnExistsAsync(Guid libraryId, Isbn isbn, Guid? excludingBookId, CancellationToken cancellationToken);
}

public interface IMemberRepository : IRepository<Member>
{
    Task<bool> EmailExistsAsync(Email email, Guid? excludingMemberId, CancellationToken cancellationToken);
}

public interface ILoanRepository : IRepository<Loan>
{
    Task<int> CountActiveByMemberAsync(Guid memberId, CancellationToken cancellationToken);
    Task<bool> HasActiveLoanAsync(Guid memberId, Guid bookId, CancellationToken cancellationToken);
    Task<bool> HasActiveLoansForMemberAsync(Guid memberId, CancellationToken cancellationToken);
}