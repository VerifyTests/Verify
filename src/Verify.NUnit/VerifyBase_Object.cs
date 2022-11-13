namespace VerifyNUnit;

public partial class VerifyBase
{
    public SettingsTask Verify<T>(
        Task<T> target,
        VerifySettings? settings = null)
        where T : notnull =>
        Verifier.Verify(target, settings ?? this.settings, sourceFile);

    public SettingsTask Verify<T>(
        ValueTask<T> target,
        VerifySettings? settings = null)
        where T : notnull =>
        Verifier.Verify(target, settings ?? this.settings, sourceFile);

    public SettingsTask Verify<T>(
        IAsyncEnumerable<T> target,
        VerifySettings? settings = null)
        where T : notnull =>
        Verifier.Verify(target, settings ?? this.settings, sourceFile);

    public SettingsTask Verify(
        object? target,
        VerifySettings? settings = null) =>
        Verifier.Verify(target, settings ?? this.settings, sourceFile);
}