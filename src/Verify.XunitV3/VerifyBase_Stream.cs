namespace VerifyXunit;

public partial class VerifyBase
{
    [Pure]
    public SettingsTask Verify(
        FileStream? target,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.Verify(target, settings ?? this.settings, info, sourceFile);

    [Pure]
    public SettingsTask Verify(
        Stream? target,
        string extension,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.Verify(target, extension, settings ?? this.settings, info, sourceFile);

    [Pure]
    public SettingsTask Verify(
        Stream? target,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.Verify(target, settings ?? this.settings, info, sourceFile);

    [Pure]
    public SettingsTask Verify<T>(
        Task<T> target,
        string extension,
        VerifySettings? settings = null,
        object? info = null)
        where T : Stream =>
        Verifier.Verify(target, extension, settings ?? this.settings, info, sourceFile);

    [Pure]
    public SettingsTask Verify<T>(
        ValueTask<T> target,
        string extension,
        VerifySettings? settings = null,
        object? info = null)
        where T : Stream =>
        Verifier.Verify(target, extension, settings ?? this.settings, info, sourceFile);

    [Pure]
    public SettingsTask Verify<T>(
        IEnumerable<T> targets,
        string extension,
        VerifySettings? settings = null,
        object? info = null)
        where T : Stream =>
        Verifier.Verify(targets, extension, settings ?? this.settings, info, sourceFile);

    [Pure]
    public SettingsTask Verify(
        byte[]? target,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.Verify(target, settings ?? this.settings, info, sourceFile);

    [Pure]
    public SettingsTask Verify(
        byte[]? target,
        string extension,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.Verify(target, extension, settings ?? this.settings, info, sourceFile);

    [Pure]
    public SettingsTask Verify(
        Task<byte[]> target,
        string extension,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.Verify(target, extension, settings ?? this.settings, info, sourceFile);

    [Pure]
    public SettingsTask Verify(
        ValueTask<byte[]> target,
        string extension,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.Verify(target, extension, settings ?? this.settings, info, sourceFile);
}