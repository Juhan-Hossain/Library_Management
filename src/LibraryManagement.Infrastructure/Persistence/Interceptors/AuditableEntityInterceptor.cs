using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LibraryManagement.Infrastructure.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor(TimeProvider clock) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Stamp(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Stamp(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Stamp(DbContext? context)
    {
        if (context is null)
            return;

        var utcNow = clock.GetUtcNow().UtcDateTime;
        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.MarkCreated(utcNow);
            else if (entry.State == EntityState.Modified)
                entry.Entity.MarkUpdated(utcNow);
        }
    }
}