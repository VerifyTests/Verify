namespace VerifyNUnit;

public partial class VerifyBase
{
    [Pure]
    public SettingsTask Verify<T>(
        Func<Task<T>> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(target(), settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask Verify<T>(
        Task<T> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask Verify<T>(
        ValueTask<T> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask Verify<T>(
        IAsyncEnumerable<T> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask Verify(
        object? target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(target, settings ?? this.settings, sourceFile, lineNumber);
}