using System.Linq.Expressions;

namespace LibraryManagement.Application.Features.Authors;

public sealed record AuthorDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string? Biography,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

internal static class AuthorProjections
{
    public static readonly Expression<Func<Author, AuthorDto>> ToDto = a =>
        new AuthorDto(a.Id, a.FirstName, a.LastName, a.FirstName + " " + a.LastName,
            a.Biography, a.CreatedAtUtc, a.UpdatedAtUtc);
}