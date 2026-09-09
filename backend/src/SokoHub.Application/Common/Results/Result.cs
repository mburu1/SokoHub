using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Common.Results;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure { get; }
    public ApplicationError Error { get; }

    protected Result(bool isSuccess, ApplicationError? error)
    {
        IsSuccess = isSuccess;
        IsFailure = !isSuccess;
        Error = error!;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(ApplicationError error) => new(false, error);

    public void ThrowIfFailure()
    {
        if (IsFailure)
        {
            throw new Exception(Error.Description);
        }
    }
}
