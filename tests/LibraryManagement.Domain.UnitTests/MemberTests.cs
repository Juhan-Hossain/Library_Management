namespace LibraryManagement.Domain.UnitTests;

public sealed class MemberTests
{
    [Fact]
    public void Create_starts_active_with_membership_date()
    {
        var member = DomainFakes.NewMember();

        member.IsActive.Should().BeTrue();
        member.MembershipDate.Should().Be(new DateOnly(2026, 1, 1));
        member.Email.Value.Should().Be("ada@example.com");
    }

    [Fact]
    public void Deactivate_then_activate_toggles_status()
    {
        var member = DomainFakes.NewMember();

        member.Deactivate();
        member.IsActive.Should().BeFalse();

        member.Activate();
        member.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_rejects_empty_library()
    {
        var act = () => Member.Create("Ada", "Lovelace", Email.Create("ada@example.com"), null, Guid.Empty, new DateOnly(2026, 1, 1));
        act.Should().Throw<DomainValidationException>();
    }
}