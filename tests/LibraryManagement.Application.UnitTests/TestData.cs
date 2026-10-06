namespace LibraryManagement.Application.UnitTests;

internal static class TestData
{
    public static readonly DateTimeOffset Now = new(2026, 10, 5, 9, 0, 0, TimeSpan.Zero);

    public static FakeTimeProvider Clock() => new(Now);

    public static Book NewBook(Guid libraryId, int copies = 1) =>
        Book.Create("Clean Code", Isbn.Create("9780132350884"), 2008, "Software", copies, libraryId, Guid.NewGuid());

    public static Member NewMember(Guid libraryId) =>
        Member.Create("Ada", "Lovelace", Email.Create("ada@example.com"), null, libraryId,
            DateOnly.FromDateTime(Now.UtcDateTime));
}