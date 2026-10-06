using System.Net.Mail;
using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.ValueObjects;

public sealed class Email : IEquatable<Email>
{
    public const int MaxLength = 256;

    private Email(string value) => Value = value;

    public string Value { get; }

    public static Email Create(string? raw) =>
        TryNormalize(raw, out var value)
            ? new Email(value)
            : throw new DomainValidationException($"'{raw}' is not a valid email address.");

    public static bool IsValid(string? raw) => TryNormalize(raw, out _);

    private static bool TryNormalize(string? raw, out string value)
    {
        value = raw?.Trim().ToLowerInvariant() ?? string.Empty;
        return value.Length is > 0 and <= MaxLength
            && MailAddress.TryCreate(value, out var parsed)
            && parsed.Address == value;
    }

    public bool Equals(Email? other) => other is not null && Value == other.Value;

    public override bool Equals(object? obj) => Equals(obj as Email);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    public override string ToString() => Value;
}