namespace LibraryManagement.Application.Features.Members;

public interface IMemberPayload
{
    string FirstName { get; }
    string LastName { get; }
    string Email { get; }
    string? Phone { get; }
    Guid LibraryId { get; }
}

internal sealed class MemberPayloadValidator : AbstractValidator<IMemberPayload>
{
    public MemberPayloadValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(Member.NameMaxLength);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(Member.NameMaxLength);
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(Email.MaxLength)
            .Must(Email.IsValid).WithMessage("'{PropertyValue}' is not a valid email address.");
        RuleFor(x => x.Phone).MaximumLength(Member.PhoneMaxLength);
        RuleFor(x => x.LibraryId).NotEmpty();
    }
}