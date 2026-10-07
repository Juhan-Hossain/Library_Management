using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Persistence.Configurations;

internal sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books", table => table.HasCheckConstraint(
            "CK_Books_AvailableCopies",
            "[AvailableCopies] >= 0 AND [AvailableCopies] <= [TotalCopies]"));

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();

        builder.Property(b => b.Title).HasMaxLength(Book.TitleMaxLength).IsRequired();
        builder.Property(b => b.Genre).HasMaxLength(Book.GenreMaxLength).IsRequired();

        // Value object <-> string column.
        builder.Property(b => b.Isbn)
            .HasConversion(isbn => isbn.Value, value => Isbn.Create(value))
            .HasMaxLength(Isbn.MaxLength)
            .IsUnicode(false)
            .IsRequired();
        builder.HasIndex(b => new { b.LibraryId, b.Isbn }).IsUnique();
        builder.HasIndex(b => b.Isbn);

        // Optimistic concurrency: UPDATE ... WHERE Version = @original.
        builder.Property(b => b.Version).IsConcurrencyToken();

        builder.Ignore(b => b.CopiesOnLoan);

        builder.HasOne(b => b.Library).WithMany().HasForeignKey(b => b.LibraryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(b => b.Author).WithMany().HasForeignKey(b => b.AuthorId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.Title);
    }
}