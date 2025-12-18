namespace System;

public abstract record Option<T>
{
    public sealed record Some(T Value) : Option<T>;
    public sealed record None() : Option<T>;

    public bool IsSome => this is Some;
    public bool IsNone => this is None;

    public T UnwrapOr(T defaultValue) => this is Some(var value) ? value : defaultValue;

    public void Match(Action<T> onSome, Action onNone)
    {
        if (this is Some(var value))
        {
            onSome(value);
        }
        else if (this is None)
        {
            onNone();
        }
    }

    public Option<T> Map(Func<T, T> f)
    {
        return this switch
        {
            Some(var value) => new Some(f(value)),
            None => new None(),
            _ => throw new InvalidOperationException("Unrecognized option type")
        };
    }

    public static implicit operator Option<T>(T value) => new Some(value);
}
