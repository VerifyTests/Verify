namespace VerifyMSTest;

partial class VerifyBase
{
#pragma warning disable CA1822 // Mark members as static

    [Pure]
    public SettingsTask Verify(
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Verify(settings, sourceFile);

    [Pure]
    public SettingsTask Verify<T>(
        Func<Task<T>> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Verify(target, settings, sourceFile);

    [Pure]
    public SettingsTask Verify<T>(
        Task<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Verify(target, settings, sourceFile);

    [Pure]
    public SettingsTask Verify<T>(
        ValueTask<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Verify(target, settings, sourceFile);

    [Pure]
    public SettingsTask Verify<T>(
        IAsyncEnumerable<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Verify(target, settings, sourceFile);

    [Pure]
    public SettingsTask Verify(
        object? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Verify(target, settings, sourceFile);
}