using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Entities;

public sealed class Library : AuditableEntity
{
    public const int NameMaxLength = 200;
    public const int AddressMaxLength = 500;
    public const int PhoneMaxLength = 30;

    private Library() { }

    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string? Phone { get; private set; }

    public static Library Create(string name, string address, string? phone)
    {
        var library = new Library();
        library.Update(name, address, phone);
        return library;
    }

    public void Update(string name, string address, string? phone)
    {
        var validName = Guard.Required(name, nameof(Name), NameMaxLength);
        var validAddress = Guard.Required(address, nameof(Address), AddressMaxLength);
        var validPhone = Guard.Optional(phone, nameof(Phone), PhoneMaxLength);

        Name = validName;
        Address = validAddress;
        Phone = validPhone;
    }
}