using FluentValidation.TestHelper;
using LibraryManagement.Application.Features.Libraries;

namespace LibraryManagement.Application.UnitTests.Libraries;

public sealed class LibraryValidatorTests
{
    [Fact]
    public void Create_validator_rejects_blank_name_and_too_long_address()
    {
        var result = new CreateLibraryCommandValidator()
            .TestValidate(new CreateLibraryCommand("", new string('a', Library.AddressMaxLength + 1), null));

        result.ShouldHaveValidationErrorFor(c => c.Name);
        result.ShouldHaveValidationErrorFor(c => c.Address);
    }

    [Fact]
    public void Create_validator_accepts_valid_command()
    {
        new CreateLibraryCommandValidator()
            .TestValidate(new CreateLibraryCommand("Central", "1 Main St", null))
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Paging_rules_reject_page_zero_and_oversized_page()
    {
        var validator = new GetLibrariesQueryValidator();

        validator.TestValidate(new GetLibrariesQuery { Page = 0 }).ShouldHaveValidationErrorFor(q => q.Page);
        validator.TestValidate(new GetLibrariesQuery { PageSize = 101 }).ShouldHaveValidationErrorFor(q => q.PageSize);
        validator.TestValidate(new GetLibrariesQuery()).ShouldNotHaveAnyValidationErrors();
    }
}