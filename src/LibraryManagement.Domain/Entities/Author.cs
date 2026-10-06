using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Entities;

public sealed class Author : AuditableEntity
{
    public const int NameMaxLength = 100;
    public const int BiographyMaxLength = 2000;

    private Author() { }

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string? Biography { get; private set; }

    public static Author Create(string firstName, string lastName, string? biography)
    {
        var author = new Author();
        author.Update(firstName, lastName, biography);
        return author;
    }

    public void Update(string firstName, string lastName, string? biography)
    {
        var validFirst = Guard.Required(firstName, nameof(FirstName), NameMaxLength);
        var validLast = Guard.Required(lastName, nameof(LastName), NameMaxLength);
        var validBio = Guard.Optional(biography, nameof(Biography), BiographyMaxLength);

        FirstName = validFirst;
        LastName = validLast;
        Biography = validBio;
    }
}