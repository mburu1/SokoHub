namespace SokoHub.Domain.Interfaces;

using SokoHub.Domain.Common.Entities;

public interface IUnitOfWork
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : Entity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
