using System.Linq.Expressions;

namespace LibraryManagement.Application.Features.Libraries;

public sealed record LibraryDto(
    Guid Id,
    string Name,
    string Address,
    string? Phone,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

internal static class LibraryProjections
{
    public static readonly Expression<Func<Library, LibraryDto>> ToDto = l =>
        new LibraryDto(l.Id, l.Name, l.Address, l.Phone, l.CreatedAtUtc, l.UpdatedAtUtc);
}