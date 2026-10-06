using System.Linq.Expressions;

namespace LibraryManagement.Application.Features.Books;

public sealed record BookDto(
    Guid Id,
    string Title,
    string Isbn,
    int PublishedYear,
    string Genre,
    int TotalCopies,
    int AvailableCopies,
    Guid LibraryId,
    string LibraryName,
    Guid AuthorId,
    string AuthorName,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

internal static class BookProjections
{
    public static readonly Expression<Func<Book, BookDto>> ToDto = b => new BookDto(
        b.Id,
        b.Title,
        b.Isbn.Value,
        b.PublishedYear,
        b.Genre,
        b.TotalCopies,
        b.AvailableCopies,
        b.LibraryId,
        b.Library!.Name,
        b.AuthorId,
        b.Author!.FirstName + " " + b.Author.LastName,
        b.CreatedAtUtc,
        b.UpdatedAtUtc);
}