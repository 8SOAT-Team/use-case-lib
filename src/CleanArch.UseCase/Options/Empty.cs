namespace CleanArch.UseCase.Options;

public record Empty<T> : Any<T>
{
    public override T Value => default!;

    public override bool HasValue => false;
}