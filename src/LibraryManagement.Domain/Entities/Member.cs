using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.ValueObjects;

namespace LibraryManagement.Domain.Entities;

public sealed class Member : AuditableEntity
{
    public const int NameMaxLength = 100;
    public const int PhoneMaxLength = 30;

    private Member() { }

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string? Phone { get; private set; }
    public DateOnly MembershipDate { get; private set; }
    public bool IsActive { get; private set; }
    public Guid LibraryId { get; private set; }

    public Library? Library { get; private set; }

    public static Member Create(string firstName, string lastName, Email email, string? phone,
        Guid libraryId, DateOnly membershipDate)
    {
        var member = new Member { MembershipDate = membershipDate, IsActive = true };
        member.Update(firstName, lastName, email, phone, libraryId);
        return member;
    }

    public void Update(string firstName, string lastName, Email email, string? phone, Guid libraryId)
    {
        ArgumentNullException.ThrowIfNull(email);
        var validFirst = Guard.Required(firstName, nameof(FirstName), NameMaxLength);
        var validLast = Guard.Required(lastName, nameof(LastName), NameMaxLength);
        var validPhone = Guard.Optional(phone, nameof(Phone), PhoneMaxLength);
        Guard.NotEmpty(libraryId, nameof(LibraryId));

        FirstName = validFirst;
        LastName = validLast;
        Email = email;
        Phone = validPhone;
        LibraryId = libraryId;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}