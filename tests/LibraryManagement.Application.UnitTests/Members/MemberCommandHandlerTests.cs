using LibraryManagement.Application.Features.Members;

namespace LibraryManagement.Application.UnitTests.Members;

public sealed class MemberCommandHandlerTests
{
    private readonly IMemberRepository _members = Substitute.For<IMemberRepository>();
    private readonly ILibraryRepository _libraries = Substitute.For<ILibraryRepository>();
    private readonly ILoanRepository _loans = Substitute.For<ILoanRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public MemberCommandHandlerTests()
    {
        _libraries.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
    }

    [Fact]
    public async Task Create_sets_membership_date_from_clock_and_saves()
    {
        var handler = new CreateMemberCommandHandler(_members, _libraries, _unitOfWork, TestData.Clock());

        var id = await handler.Handle(
            new CreateMemberCommand("Ada", "Lovelace", " Ada@Example.com ", null, Guid.NewGuid()),
            CancellationToken.None);

        _members.Received(1).Add(Arg.Is<Member>(m =>
            m.Id == id &&
            m.Email.Value == "ada@example.com" &&
            m.MembershipDate == new DateOnly(2026, 10, 5) &&
            m.IsActive));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_with_duplicate_email_throws_conflict()
    {
        _members.EmailExistsAsync(Arg.Any<Email>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(true);
        var handler = new CreateMemberCommandHandler(_members, _libraries, _unitOfWork, TestData.Clock());

        var act = () => handler.Handle(
            new CreateMemberCommand("Ada", "Lovelace", "ADA@example.com", null, Guid.NewGuid()),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>().WithMessage("*ada@example.com*");
        _members.DidNotReceive().Add(Arg.Any<Member>());
    }

    [Fact]
    public async Task Update_checks_email_uniqueness_excluding_itself_and_saves()
    {
        var member = TestData.NewMember(Guid.NewGuid());
        _members.GetByIdAsync(member.Id, Arg.Any<CancellationToken>()).Returns(member);
        var handler = new UpdateMemberCommandHandler(_members, _libraries, _unitOfWork);

        await handler.Handle(
            new UpdateMemberCommand(member.Id, "Augusta", "King", "ada@example.com", "555-0101", member.LibraryId),
            CancellationToken.None);

        await _members.Received(1).EmailExistsAsync(Arg.Any<Email>(), member.Id, Arg.Any<CancellationToken>());
        member.FirstName.Should().Be("Augusta");
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_member_with_active_loans_throws_conflict()
    {
        var member = TestData.NewMember(Guid.NewGuid());
        _members.GetByIdAsync(member.Id, Arg.Any<CancellationToken>()).Returns(member);
        _loans.HasActiveLoansForMemberAsync(member.Id, Arg.Any<CancellationToken>()).Returns(true);
        var handler = new DeleteMemberCommandHandler(_members, _loans, _unitOfWork);

        var act = () => handler.Handle(new DeleteMemberCommand(member.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
        _members.DidNotReceive().Remove(Arg.Any<Member>());
    }

    [Fact]
    public async Task Set_status_false_deactivates_and_saves()
    {
        var member = TestData.NewMember(Guid.NewGuid());
        _members.GetByIdAsync(member.Id, Arg.Any<CancellationToken>()).Returns(member);
        var handler = new SetMemberStatusCommandHandler(_members, _unitOfWork);

        await handler.Handle(new SetMemberStatusCommand(member.Id, IsActive: false), CancellationToken.None);

        member.IsActive.Should().BeFalse();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}