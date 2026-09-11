using MediatR;

namespace SokoHub.Contracts.Commands;

public abstract record IntegrationCommand(Guid CommandId) : IRequest
{
    protected IntegrationCommand() : this(Guid.NewGuid()) { }
}

public abstract record IntegrationCommand<TResponse>(Guid CommandId) : IRequest<TResponse>
{
    protected IntegrationCommand() : this(Guid.NewGuid()) { }
}
