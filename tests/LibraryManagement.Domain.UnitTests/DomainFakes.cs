namespace LibraryManagement.Domain.UnitTests;

internal static class DomainFakes
{
    public static readonly Guid LibraryId = Guid.NewGuid();
    public static readonly DateTime Now = new(2026, 10, 5, 9, 0, 0, DateTimeKind.Utc);

    public static Book NewBook(int copies = 2, Guid? libraryId = null) =>
        Book.Create("Clean Code", Isbn.Create("9780132350884"), 2008, "Software Engineering",
            copies, libraryId ?? LibraryId, Guid.NewGuid());

    public static Member NewMember(Guid? libraryId = null) =>
        Member.Create("Ada", "Lovelace", Email.Create("ada@example.com"), null,
            libraryId ?? LibraryId, new DateOnly(2026, 1, 1));
}