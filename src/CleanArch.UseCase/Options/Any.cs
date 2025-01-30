namespace CleanArch.UseCase.Options;

public abstract record Any<T>
{
    public abstract T? Value { get; }

    public abstract bool HasValue { get; }

    public static Any<T> Some(T value) => new Some<T>(value);
    public static Any<T> Empty => new Empty<T>();
}