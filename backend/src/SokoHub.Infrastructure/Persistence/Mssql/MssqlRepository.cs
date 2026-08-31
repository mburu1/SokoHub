using Microsoft.EntityFrameworkCore;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Common.Specifications;

namespace SokoHub.Infrastructure.Persistence.Mssql;

public class MssqlRepository<TEntity> : IRepository<TEntity>, IReadRepository<TEntity>, IWriteRepository<TEntity>
    where TEntity : class
{
    protected readonly SokoHubDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public MssqlRepository(SokoHubDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // This assumes entities have an 'Id' property via a base class or interface
        // For simplicity in a generic repo, we use EF's FindAsync
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await spec.ApplySpecification(_dbSet).ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> SingleAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await spec.ApplySpecification(_dbSet).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual async Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await spec.ApplySpecification(_dbSet).CountAsync(cancellationToken);
    }
}
