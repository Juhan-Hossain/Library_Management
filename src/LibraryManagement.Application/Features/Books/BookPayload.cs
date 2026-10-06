namespace LibraryManagement.Application.Features.Books;

public interface IBookPayload
{
    string Title { get; }
    string Isbn { get; }
    int PublishedYear { get; }
    string Genre { get; }
    int TotalCopies { get; }
    Guid LibraryId { get; }
    Guid AuthorId { get; }
}

internal sealed class BookPayloadValidator : AbstractValidator<IBookPayload>
{
    public const int MaxCopies = 1000;

    public BookPayloadValidator(TimeProvider clock)
    {
        var latestYear = clock.GetUtcNow().Year + 1;

        RuleFor(x => x.Title).NotEmpty().MaximumLength(Book.TitleMaxLength);
        RuleFor(x => x.Isbn)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(Isbn.IsValid).WithMessage("'{PropertyValue}' is not a valid ISBN-10 or ISBN-13.");
        RuleFor(x => x.PublishedYear).InclusiveBetween(Book.EarliestPublishedYear, latestYear);
        RuleFor(x => x.Genre).NotEmpty().MaximumLength(Book.GenreMaxLength);
        RuleFor(x => x.TotalCopies).InclusiveBetween(1, MaxCopies);
        RuleFor(x => x.LibraryId).NotEmpty();
        RuleFor(x => x.AuthorId).NotEmpty();
    }
}