using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Common.Results;

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, ApplicationError? error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value cannot be accessed when the result is a failure.");

    public static Result<TValue> Success(TValue value) => new(value, true, null);
    public static Result<TValue> Failure(ApplicationError error) => new(default, false, error);
}
