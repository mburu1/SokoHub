using Microsoft.EntityFrameworkCore.Storage;

namespace SokoHub.Infrastructure.Persistence.Mssql;

public class MssqlTransactionManager
{
    private readonly SokoHubDbContext _context;

    public MssqlTransactionManager(SokoHubDbContext context)
    {
        _context = context;
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await action();
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
