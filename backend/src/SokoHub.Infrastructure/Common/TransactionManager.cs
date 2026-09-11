using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Infrastructure.Persistence.Mssql;

namespace SokoHub.Infrastructure.Common;

public class TransactionManager : ITransactionManager
{
    private readonly SokoHubDbContext _context;

    public TransactionManager(SokoHubDbContext context)
    {
        _context = context;
    }

    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return transaction.GetDbTransaction();
    }
}
