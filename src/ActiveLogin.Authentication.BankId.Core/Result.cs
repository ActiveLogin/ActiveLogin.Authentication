namespace System;

public abstract record Result<T>
{
    public static Result<T> From(T value, string? errorMessage)
    {
        return errorMessage is null
            ? new Success(value)
            : new Failure(errorMessage);
    }

    public sealed record Success(T Value) : Result<T>;
    public sealed record Failure(string ErrorMessage) : Result<T>;

    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;

    /// <summary>
    /// Maps the result value if success, otherwise propagates the failure.
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="f"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Result<TOut> Map<TOut>(Func<T, TOut> f)
    {
        return this switch
        {
            Success(var value) => new Result<TOut>.Success(f(value)),
            Failure(var errorMessage) => new Result<TOut>.Failure(errorMessage),
            _ => throw new InvalidOperationException("Unrecognized result type")
        };
    }

    /// <summary>
    /// Pattern match on the result.
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onFailure"></param>
    public void Match(Action<T> onSuccess, Action<string> onFailure)
    {
        if (this is Success(var value))
        {
            onSuccess(value);
        }
        else if (this is Failure(var errorMessage))
        {
            onFailure(errorMessage);
        }
    }

    /// <summary>
    /// Unwraps the result value if success, otherwise returns the provided default value.
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public T UnwrapOr(T defaultValue) => this is Success(var value) ? value : defaultValue;

    public static implicit operator Result<T>(T value) => new Success(value);
    public static implicit operator Result<T>(string errorMessage) => new Failure(errorMessage);
}
