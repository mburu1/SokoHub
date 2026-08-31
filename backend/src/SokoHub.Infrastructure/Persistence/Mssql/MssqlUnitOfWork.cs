using Microsoft.EntityFrameworkCore;
using SokoHub.Domain.Interfaces;

namespace SokoHub.Infrastructure.Persistence.Mssql;

public class MssqlUnitOfWork : IUnitOfWork
{
    private readonly SokoHubDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();

    public MssqlUnitOfWork(SokoHubDbContext context)
    {
        _context = context;
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new MssqlRepository<TEntity>(_context);
        }
        return (IRepository<TEntity>)_repositories[type];
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.RollbackTransactionAsync(cancellationToken);
    }
}
