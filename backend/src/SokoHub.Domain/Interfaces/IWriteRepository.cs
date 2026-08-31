namespace SokoHub.Domain.Interfaces;

public interface IWriteRepository<T>
    where T : AggregateRoot
{
    Task AddAsync(T aggregate, CancellationToken cancellationToken = default);

    void Update(T aggregate);

    void Remove(T aggregate);
}
