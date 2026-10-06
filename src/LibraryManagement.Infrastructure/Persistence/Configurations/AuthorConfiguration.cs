using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Persistence.Configurations;

internal sealed class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("Authors");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.FirstName).HasMaxLength(Author.NameMaxLength).IsRequired();
        builder.Property(a => a.LastName).HasMaxLength(Author.NameMaxLength).IsRequired();
        builder.Property(a => a.Biography).HasMaxLength(Author.BiographyMaxLength);

        builder.HasIndex(a => new { a.LastName, a.FirstName });
    }
}