namespace LibraryManagement.Application.Features.Authors;

public interface IAuthorPayload
{
    string FirstName { get; }
    string LastName { get; }
    string? Biography { get; }
}

internal sealed class AuthorPayloadValidator : AbstractValidator<IAuthorPayload>
{
    public AuthorPayloadValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(Author.NameMaxLength);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(Author.NameMaxLength);
        RuleFor(x => x.Biography).MaximumLength(Author.BiographyMaxLength);
    }
}