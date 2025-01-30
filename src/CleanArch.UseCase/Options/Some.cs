namespace CleanArch.UseCase.Options;

public record Some<T> : Any<T>
{
    public Some(T value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));

        Value = value;
    }

    public override T Value { get; }

    public override bool HasValue => true;
}