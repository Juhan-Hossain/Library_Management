namespace LibraryManagement.Api.Contracts;

public sealed record UpdateLibraryRequest(string Name, string Address, string? Phone);

public sealed record UpdateAuthorRequest(string FirstName, string LastName, string? Biography);

public sealed record UpdateBookRequest(
    string Title,
    string Isbn,
    int PublishedYear,
    string Genre,
    int TotalCopies,
    Guid LibraryId,
    Guid AuthorId);

public sealed record UpdateMemberRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    Guid LibraryId);