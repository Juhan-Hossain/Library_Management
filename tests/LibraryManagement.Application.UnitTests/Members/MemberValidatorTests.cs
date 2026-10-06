using FluentValidation.TestHelper;
using LibraryManagement.Application.Features.Members;

namespace LibraryManagement.Application.UnitTests.Members;

public sealed class MemberValidatorTests
{
    private readonly CreateMemberCommandValidator _validator = new();

    [Fact]
    public void Valid_command_passes()
    {
        _validator.TestValidate(new CreateMemberCommand("Ada", "Lovelace", "ada@example.com", null, Guid.NewGuid()))
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Invalid_email_and_missing_library_fail()
    {
        var result = _validator.TestValidate(new CreateMemberCommand("Ada", "Lovelace", "not-an-email", null, Guid.Empty));

        result.ShouldHaveValidationErrorFor(c => c.Email);
        result.ShouldHaveValidationErrorFor(c => c.LibraryId);
    }
}