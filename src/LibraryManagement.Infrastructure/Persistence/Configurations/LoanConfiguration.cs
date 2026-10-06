using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Persistence.Configurations;

internal sealed class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedNever();

        builder.Property(l => l.Status).HasConversion<string>().HasMaxLength(20);

        // Deleting a book/member (only allowed when no ACTIVE loans) removes its returned-loan history.
        builder.HasOne(l => l.Book).WithMany().HasForeignKey(l => l.BookId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(l => l.Member).WithMany().HasForeignKey(l => l.MemberId).OnDelete(DeleteBehavior.Cascade);

        // Supports "active loans for member" and "active loan for member+book" lookups.
        builder.HasIndex(l => new { l.MemberId, l.Status });
        builder.HasIndex(l => new { l.BookId, l.Status });
    }
}