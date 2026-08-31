namespace SokoHub.Domain.Common.Exceptions;

public sealed class DomainValidationException : DomainException
{
    public DomainValidationException(string code, string message)
        : base(code, message)
    {
    }
}
