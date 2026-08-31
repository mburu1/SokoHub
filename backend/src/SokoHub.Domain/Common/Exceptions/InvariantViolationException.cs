namespace SokoHub.Domain.Common.Exceptions;

public sealed class InvariantViolationException : DomainException
{
    public InvariantViolationException(string code, string message)
        : base(code, message)
    {
    }
}
