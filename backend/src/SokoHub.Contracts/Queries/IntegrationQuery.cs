using MediatR;

namespace SokoHub.Contracts.Queries;

public abstract record IntegrationQuery<TResponse>(Guid QueryId) : IRequest<TResponse>
{
    protected IntegrationQuery() : this(Guid.NewGuid()) { }
}
