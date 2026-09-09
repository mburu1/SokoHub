namespace SokoHub.Domain.Interfaces;

using SokoHub.Domain.Common.Entities;

public interface IUnitOfWork
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : Entity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
