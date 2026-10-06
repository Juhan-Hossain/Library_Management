namespace LibraryManagement.Domain.Common;

internal static class Guard
{
    public static string Required(string? value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException($"{field} is required.");

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new DomainValidationException($"{field} must be at most {maxLength} characters.");

        return trimmed;
    }

    public static string? Optional(string? value, string field, int maxLength) =>
        string.IsNullOrWhiteSpace(value) ? null : Required(value, field, maxLength);

    public static void NotEmpty(Guid value, string field)
    {
        if (value == Guid.Empty)
            throw new DomainValidationException($"{field} is required.");
    }

    public static void Positive(int value, string field)
    {
        if (value <= 0)
            throw new DomainValidationException($"{field} must be greater than zero.");
    }
}