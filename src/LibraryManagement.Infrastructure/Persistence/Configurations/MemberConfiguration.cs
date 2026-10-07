using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Persistence.Configurations;

internal sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.FirstName).HasMaxLength(Member.NameMaxLength).IsRequired();
        builder.Property(m => m.LastName).HasMaxLength(Member.NameMaxLength).IsRequired();
        builder.Property(m => m.Phone).HasMaxLength(Member.PhoneMaxLength);

        builder.Property(m => m.Email)
            .HasConversion(email => email.Value, value => Email.Create(value))
            .HasMaxLength(Email.MaxLength)
            .IsRequired();
        builder.HasIndex(m => m.Email).IsUnique();

        builder.HasOne(m => m.Library).WithMany().HasForeignKey(m => m.LibraryId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.LastName, m.FirstName });
        builder.Property(m => m.Version).IsConcurrencyToken();
    }
}