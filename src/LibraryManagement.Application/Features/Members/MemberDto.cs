using System.Linq.Expressions;

namespace LibraryManagement.Application.Features.Members;

public sealed record MemberDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string? Phone,
    DateOnly MembershipDate,
    bool IsActive,
    Guid LibraryId,
    string LibraryName,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

internal static class MemberProjections
{
    public static readonly Expression<Func<Member, MemberDto>> ToDto = m => new MemberDto(
        m.Id,
        m.FirstName,
        m.LastName,
        m.FirstName + " " + m.LastName,
        m.Email.Value,
        m.Phone,
        m.MembershipDate,
        m.IsActive,
        m.LibraryId,
        m.Library!.Name,
        m.CreatedAtUtc,
        m.UpdatedAtUtc);
}