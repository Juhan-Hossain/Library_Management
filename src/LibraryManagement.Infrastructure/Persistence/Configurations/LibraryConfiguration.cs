using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Persistence.Configurations;

internal sealed class LibraryConfiguration : IEntityTypeConfiguration<Library>
{
    public void Configure(EntityTypeBuilder<Library> builder)
    {
        builder.ToTable("Libraries");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedNever();

        builder.Property(l => l.Name).HasMaxLength(Library.NameMaxLength).IsRequired();
        builder.Property(l => l.Address).HasMaxLength(Library.AddressMaxLength).IsRequired();
        builder.Property(l => l.Phone).HasMaxLength(Library.PhoneMaxLength);

        builder.HasIndex(l => l.Name);
    }
}