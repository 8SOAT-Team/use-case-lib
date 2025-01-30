namespace CleanArch.UseCase.Options;

public static class AnyExtension
{
    public static Any<T> ToAny<T>(this T? value) where T : class
        =>  value is null || (value is string s && string.IsNullOrEmpty(s)) ?
            Any<T>.Empty : Any<T>.Some(value);
}