using Microsoft.EntityFrameworkCore;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Domain.Interfaces;

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
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await spec.ApplySpecification(_dbSet).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> SingleAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await spec.ApplySpecification(_dbSet).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> SingleAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await spec.ApplySpecification(_dbSet).ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await spec.ApplySpecification(_dbSet).CountAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await spec.ApplySpecification(_dbSet).AnyAsync(cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await spec.ApplySpecification(_dbSet).AnyAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default)
    {
        var id = Guid.TryParse(reference, out var parsed) ? parsed : default;
        return id == Guid.Empty ? null : await _dbSet.FindAsync(new object[] { id }, cancellationToken);
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
}
