namespace LibraryManagement.Domain.UnitTests;

public sealed class EmailTests
{
    [Fact]
    public void Create_trims_and_lowercases()
    {
        Email.Create("  Ada.Lovelace@Example.COM ").Value.Should().Be("ada.lovelace@example.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("a@b@c.com")]
    [InlineData("Ada Lovelace <ada@example.com>")]
    public void Create_rejects_invalid_email(string? raw)
    {
        var act = () => Email.Create(raw);
        act.Should().Throw<DomainValidationException>();
        Email.IsValid(raw).Should().BeFalse();
    }

    [Fact]
    public void Emails_differing_only_by_case_are_equal()
    {
        Email.Create("ADA@example.com").Should().Be(Email.Create("ada@EXAMPLE.com"));
    }
}