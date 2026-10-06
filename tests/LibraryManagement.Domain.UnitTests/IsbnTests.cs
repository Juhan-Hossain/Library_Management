namespace LibraryManagement.Domain.UnitTests;

public sealed class IsbnTests
{
    [Theory]
    [InlineData("9780132350884", "9780132350884")]
    [InlineData("978-0-13-235088-4", "9780132350884")]
    [InlineData(" 978 0 13 235088 4 ", "9780132350884")]
    [InlineData("0132350882", "0132350882")]
    [InlineData("0-8044-2957-x", "080442957X")]
    public void Create_normalises_valid_isbn(string raw, string expected)
    {
        Isbn.Create(raw).Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("9780132350885")]
    [InlineData("0132350883")]
    [InlineData("97801323508AB")]
    public void Create_rejects_invalid_isbn(string? raw)
    {
        var act = () => Isbn.Create(raw);
        act.Should().Throw<DomainValidationException>();
        Isbn.IsValid(raw).Should().BeFalse();
    }

    [Fact]
    public void Isbns_with_same_value_are_equal()
    {
        Isbn.Create("978-0-13-235088-4").Should().Be(Isbn.Create("9780132350884"));
        Isbn.Create("9780132350884").GetHashCode().Should().Be(Isbn.Create("9780132350884").GetHashCode());
    }
}