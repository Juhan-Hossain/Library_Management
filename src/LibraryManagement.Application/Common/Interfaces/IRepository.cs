using LibraryManagement.Domain.Common;

namespace LibraryManagement.Application.Common.Interfaces;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
    void Add(TEntity entity);
    void Remove(TEntity entity);
}