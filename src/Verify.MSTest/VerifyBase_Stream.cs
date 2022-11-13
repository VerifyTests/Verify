namespace VerifyMSTest;

public partial class VerifyBase
{
    public SettingsTask Verify(
        byte[]? target,
        string extension,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, extension, info));

    public SettingsTask Verify(
        byte[]? target,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, info));

    public SettingsTask Verify(
        Task<byte[]> target,
        string extension,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, extension, info));

    public SettingsTask Verify(
        ValueTask<byte[]> target,
        string extension,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, extension, info));

    public SettingsTask Verify(
        FileStream? target,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, info));

    public SettingsTask Verify(
        Stream? target,
        string extension,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, extension, info));

    public SettingsTask Verify(
        Stream? target,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, info));

    public SettingsTask Verify<T>(
        Task<T> target,
        string extension,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
        where T : Stream =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, extension, info));

    public SettingsTask Verify<T>(
        ValueTask<T> target,
        string extension,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
        where T : Stream =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, extension, info));

    public SettingsTask Verify<T>(
        IEnumerable<T> targets,
        string extension,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
        where T : Stream =>
        Verify(settings, sourceFile, _ => _.VerifyStreams(targets, extension, info));
}