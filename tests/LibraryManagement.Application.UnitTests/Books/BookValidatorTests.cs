using FluentValidation.TestHelper;
using LibraryManagement.Application.Features.Books;

namespace LibraryManagement.Application.UnitTests.Books;

public sealed class BookValidatorTests
{
    // TestData.Now is 2026-10-05, so the latest allowed published year is 2027.
    private readonly CreateBookCommandValidator _validator = new(TestData.Clock());

    private static CreateBookCommand Valid() =>
        new("Clean Code", "9780132350884", 2008, "Software", 2, Guid.NewGuid(), Guid.NewGuid());

    [Fact]
    public void Valid_command_passes()
    {
        _validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("123")]
    [InlineData("9780132350885")]
    public void Invalid_isbn_fails(string isbn)
    {
        _validator.TestValidate(Valid() with { Isbn = isbn }).ShouldHaveValidationErrorFor(c => c.Isbn);
    }

    [Fact]
    public void Published_year_allows_next_year_but_not_later()
    {
        _validator.TestValidate(Valid() with { PublishedYear = 2027 }).ShouldNotHaveValidationErrorFor(c => c.PublishedYear);
        _validator.TestValidate(Valid() with { PublishedYear = 2028 }).ShouldHaveValidationErrorFor(c => c.PublishedYear);
    }

    [Fact]
    public void Zero_copies_fails()
    {
        _validator.TestValidate(Valid() with { TotalCopies = 0 }).ShouldHaveValidationErrorFor(c => c.TotalCopies);
    }

    [Fact]
    public void Get_books_rejects_malformed_isbn_filter()
    {
        new GetBooksQueryValidator()
            .TestValidate(new GetBooksQuery { Isbn = "not-an-isbn" })
            .ShouldHaveValidationErrorFor(q => q.Isbn);
    }
}