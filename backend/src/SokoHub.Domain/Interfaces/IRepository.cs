namespace SokoHub.Domain.Interfaces;

public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T>
    where T : Entity
{
    Task<T?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);
}
