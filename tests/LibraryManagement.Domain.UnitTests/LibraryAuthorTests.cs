namespace LibraryManagement.Domain.UnitTests;

public sealed class LibraryAuthorTests
{
    [Fact]
    public void Library_create_trims_values_and_treats_blank_phone_as_null()
    {
        var library = Library.Create("  Central  ", " 1 Main St ", "   ");

        library.Name.Should().Be("Central");
        library.Address.Should().Be("1 Main St");
        library.Phone.Should().BeNull();
        library.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Library_rejects_blank_name()
    {
        var act = () => Library.Create(" ", "Address", null);
        act.Should().Throw<DomainValidationException>().WithMessage("Name is required.");
    }

    [Fact]
    public void Library_update_is_atomic_when_a_later_field_is_invalid()
    {
        var library = Library.Create("Central", "1 Main St", null);

        var act = () => library.Update("Renamed", "", null);

        act.Should().Throw<DomainValidationException>();
        library.Name.Should().Be("Central");
    }

    [Fact]
    public void Author_rejects_name_longer_than_max()
    {
        var act = () => Author.Create(new string('a', Author.NameMaxLength + 1), "Martin", null);
        act.Should().Throw<DomainValidationException>();
    }
}