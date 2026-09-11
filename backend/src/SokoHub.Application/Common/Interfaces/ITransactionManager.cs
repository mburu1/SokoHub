using System.Data;

namespace SokoHub.Application.Common.Interfaces;

public interface ITransactionManager
{
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
