namespace LibraryManagement.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public void MarkCreated(DateTime utcNow) => CreatedAtUtc = utcNow;

    public void MarkUpdated(DateTime utcNow) => UpdatedAtUtc = utcNow;
}