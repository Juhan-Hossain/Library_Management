namespace LibraryManagement.Application.Features.Libraries;

public interface ILibraryPayload
{
    string Name { get; }
    string Address { get; }
    string? Phone { get; }
}

internal sealed class LibraryPayloadValidator : AbstractValidator<ILibraryPayload>
{
    public LibraryPayloadValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Library.NameMaxLength);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(Library.AddressMaxLength);
        RuleFor(x => x.Phone).MaximumLength(Library.PhoneMaxLength);
    }
}