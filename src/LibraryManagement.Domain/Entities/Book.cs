using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.ValueObjects;

namespace LibraryManagement.Domain.Entities;

public sealed class Book : AuditableEntity
{
    public const int TitleMaxLength = 300;
    public const int GenreMaxLength = 100;
    public const int EarliestPublishedYear = 1450;

    private Book() { }

    public string Title { get; private set; } = null!;
    public Isbn Isbn { get; private set; } = null!;
    public int PublishedYear { get; private set; }
    public string Genre { get; private set; } = null!;
    public int TotalCopies { get; private set; }
    public int AvailableCopies { get; private set; }
    public Guid LibraryId { get; private set; }
    public Guid AuthorId { get; private set; }

    /// <summary>Optimistic-concurrency token; regenerated on every state change.</summary>
    public Guid Version { get; private set; }

    public Library? Library { get; private set; }
    public Author? Author { get; private set; }

    public int CopiesOnLoan => TotalCopies - AvailableCopies;

    public static Book Create(string title, Isbn isbn, int publishedYear, string genre,
        int totalCopies, Guid libraryId, Guid authorId)
    {
        var book = new Book();
        book.Update(title, isbn, publishedYear, genre, totalCopies, libraryId, authorId);
        return book;
    }

    public void Update(string title, Isbn isbn, int publishedYear, string genre,
        int totalCopies, Guid libraryId, Guid authorId)
    {
        ArgumentNullException.ThrowIfNull(isbn);
        var validTitle = Guard.Required(title, nameof(Title), TitleMaxLength);
        var validGenre = Guard.Required(genre, nameof(Genre), GenreMaxLength);
        Guard.Positive(totalCopies, nameof(TotalCopies));
        Guard.NotEmpty(libraryId, nameof(LibraryId));
        Guard.NotEmpty(authorId, nameof(AuthorId));
        if (publishedYear < EarliestPublishedYear)
            throw new DomainValidationException($"{nameof(PublishedYear)} must be {EarliestPublishedYear} or later.");

        var onLoan = CopiesOnLoan;
        if (totalCopies < onLoan)
            throw new BusinessRuleViolationException(
                $"{nameof(TotalCopies)} cannot be less than the {onLoan} copies currently on loan.");
        if (onLoan > 0 && libraryId != LibraryId)
            throw new BusinessRuleViolationException("A book cannot move to another library while copies are on loan.");

        Title = validTitle;
        Isbn = isbn;
        PublishedYear = publishedYear;
        Genre = validGenre;
        TotalCopies = totalCopies;
        AvailableCopies = totalCopies - onLoan;
        LibraryId = libraryId;
        AuthorId = authorId;
        Version = Guid.NewGuid();
    }

    public void CheckOut()
    {
        if (AvailableCopies == 0)
            throw new BusinessRuleViolationException($"No copies of '{Title}' are currently available.");

        AvailableCopies--;
        Version = Guid.NewGuid();
    }

    public void ReturnCopy()
    {
        if (AvailableCopies >= TotalCopies)
            throw new BusinessRuleViolationException($"All copies of '{Title}' are already in the library.");

        AvailableCopies++;
        Version = Guid.NewGuid();
    }
}