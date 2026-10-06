namespace LibraryManagement.Domain.UnitTests;

public sealed class BookTests
{
    [Fact]
    public void Create_makes_all_copies_available()
    {
        var book = DomainFakes.NewBook(copies: 3);

        book.TotalCopies.Should().Be(3);
        book.AvailableCopies.Should().Be(3);
        book.CopiesOnLoan.Should().Be(0);
        book.Version.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_rejects_non_positive_copies(int copies)
    {
        var act = () => DomainFakes.NewBook(copies);
        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void Create_rejects_year_before_printing_press()
    {
        var act = () => Book.Create("Old", Isbn.Create("9780132350884"), 1449, "History", 1, Guid.NewGuid(), Guid.NewGuid());
        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void CheckOut_decrements_available_and_changes_version()
    {
        var book = DomainFakes.NewBook(copies: 2);
        var version = book.Version;

        book.CheckOut();

        book.AvailableCopies.Should().Be(1);
        book.CopiesOnLoan.Should().Be(1);
        book.Version.Should().NotBe(version);
    }

    [Fact]
    public void CheckOut_with_no_copies_left_throws()
    {
        var book = DomainFakes.NewBook(copies: 1);
        book.CheckOut();

        var act = () => book.CheckOut();

        act.Should().Throw<BusinessRuleViolationException>().WithMessage("*No copies*");
    }

    [Fact]
    public void ReturnCopy_when_all_copies_present_throws()
    {
        var book = DomainFakes.NewBook(copies: 1);

        var act = () => book.ReturnCopy();

        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void Update_recalculates_available_from_copies_on_loan()
    {
        var book = DomainFakes.NewBook(copies: 3);
        book.CheckOut();

        book.Update(book.Title, book.Isbn, book.PublishedYear, book.Genre, 5, book.LibraryId, book.AuthorId);

        book.TotalCopies.Should().Be(5);
        book.AvailableCopies.Should().Be(4);
    }

    [Fact]
    public void Update_total_below_copies_on_loan_throws()
    {
        var book = DomainFakes.NewBook(copies: 3);
        book.CheckOut();
        book.CheckOut();

        var act = () => book.Update(book.Title, book.Isbn, book.PublishedYear, book.Genre, 1, book.LibraryId, book.AuthorId);

        act.Should().Throw<BusinessRuleViolationException>();
        book.TotalCopies.Should().Be(3);
    }

    [Fact]
    public void Update_total_equal_to_copies_on_loan_leaves_none_available()
    {
        var book = DomainFakes.NewBook(copies: 3);
        book.CheckOut();

        book.Update(book.Title, book.Isbn, book.PublishedYear, book.Genre, 1, book.LibraryId, book.AuthorId);

        book.AvailableCopies.Should().Be(0);
    }

    [Fact]
    public void Moving_library_while_on_loan_throws()
    {
        var book = DomainFakes.NewBook(copies: 2);
        book.CheckOut();

        var act = () => book.Update(book.Title, book.Isbn, book.PublishedYear, book.Genre, 2, Guid.NewGuid(), book.AuthorId);

        act.Should().Throw<BusinessRuleViolationException>();
    }
}