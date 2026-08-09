namespace VerifyXunit;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask Verify(
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.Verify());

    [Pure]
    public static SettingsTask Verify<T>(
        Func<Task<T>> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.Verify(target()));

    [Pure]
    public static SettingsTask Verify<T>(
        Task<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.Verify(target));

    [Pure]
    public static SettingsTask Verify<T>(
        ValueTask<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.Verify(target));

    [Pure]
    public static SettingsTask Verify<T>(
        IAsyncEnumerable<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.Verify(target));

    [Pure]
    public static SettingsTask Verify(
        object? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.Verify(target));
}