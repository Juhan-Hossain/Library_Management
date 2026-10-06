using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.ValueObjects;

/// <summary>ISBN-10 or ISBN-13, stored without separators (e.g. "9780132350884").</summary>
public sealed class Isbn : IEquatable<Isbn>
{
    public const int MaxLength = 13;

    private Isbn(string value) => Value = value;

    public string Value { get; }

    public static Isbn Create(string? raw)
    {
        var normalized = Normalize(raw);
        if (!IsValidNormalized(normalized))
            throw new DomainValidationException($"'{raw}' is not a valid ISBN-10 or ISBN-13.");

        return new Isbn(normalized);
    }

    public static bool IsValid(string? raw) => IsValidNormalized(Normalize(raw));

    private static string Normalize(string? raw) =>
        raw is null
            ? string.Empty
            : new string(raw.Where(c => c is not ('-' or ' ')).ToArray()).ToUpperInvariant();

    private static bool IsValidNormalized(string value) => value.Length switch
    {
        10 => IsValidIsbn10(value),
        13 => IsValidIsbn13(value),
        _ => false
    };

    private static bool IsValidIsbn10(string value)
    {
        var sum = 0;
        for (var i = 0; i < 10; i++)
        {
            int digit;
            if (i == 9 && value[i] == 'X')
                digit = 10;
            else if (char.IsAsciiDigit(value[i]))
                digit = value[i] - '0';
            else
                return false;

            sum += digit * (10 - i);
        }

        return sum % 11 == 0;
    }

    private static bool IsValidIsbn13(string value)
    {
        if (!value.All(char.IsAsciiDigit))
            return false;

        var sum = 0;
        for (var i = 0; i < 13; i++)
            sum += (value[i] - '0') * (i % 2 == 0 ? 1 : 3);

        return sum % 10 == 0;
    }

    public bool Equals(Isbn? other) => other is not null && Value == other.Value;

    public override bool Equals(object? obj) => Equals(obj as Isbn);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    public override string ToString() => Value;
}