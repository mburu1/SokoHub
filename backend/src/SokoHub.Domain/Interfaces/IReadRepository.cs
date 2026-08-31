namespace SokoHub.Domain.Interfaces;

public interface IReadRepository<T>
    where T : AggregateRoot
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
}
