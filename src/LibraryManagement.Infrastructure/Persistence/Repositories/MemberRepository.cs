namespace LibraryManagement.Infrastructure.Persistence.Repositories;

internal sealed class MemberRepository(LibraryDbContext dbContext)
    : Repository<Member>(dbContext), IMemberRepository
{
    public Task<bool> EmailExistsAsync(Email email, Guid? excludingMemberId, CancellationToken cancellationToken) =>
        Set.AnyAsync(m => m.Email == email && m.Id != excludingMemberId, cancellationToken);
}