namespace LibraryManagement.Infrastructure.Persistence.Repositories;

internal class Repository<TEntity>(LibraryDbContext dbContext) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected LibraryDbContext DbContext { get; } = dbContext;

    protected DbSet<TEntity> Set => DbContext.Set<TEntity>();

    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Set.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken) =>
        Set.AnyAsync(e => e.Id == id, cancellationToken);

    public void Add(TEntity entity) => Set.Add(entity);

    public void Remove(TEntity entity) => Set.Remove(entity);
}