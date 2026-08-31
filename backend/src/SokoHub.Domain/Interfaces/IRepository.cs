namespace SokoHub.Domain.Interfaces;

public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T>
    where T : AggregateRoot
{
}
