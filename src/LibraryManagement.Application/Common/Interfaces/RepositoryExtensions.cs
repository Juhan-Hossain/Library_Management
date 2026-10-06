using LibraryManagement.Domain.Common;

namespace LibraryManagement.Application.Common.Interfaces;

public static class RepositoryExtensions
{
    public static async Task<TEntity> GetRequiredAsync<TEntity>(this IRepository<TEntity> repository, Guid id,
        CancellationToken cancellationToken) where TEntity : BaseEntity =>
        await repository.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(typeof(TEntity).Name, id);

    public static async Task EnsureExistsAsync<TEntity>(this IRepository<TEntity> repository, Guid id,
        CancellationToken cancellationToken) where TEntity : BaseEntity
    {
        if (!await repository.ExistsAsync(id, cancellationToken))
            throw new NotFoundException(typeof(TEntity).Name, id);
    }
}