namespace VerifyNUnit;

public static partial class Verifier
{
    public static SettingsTask Verify(
        byte[] bytes,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(bytes, extension));

    public static SettingsTask Verify(
        Task<byte[]> bytes,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(bytes, extension));

    public static SettingsTask Verify(
        FileStream stream,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(stream));

    public static SettingsTask Verify(
        Task<FileStream> stream,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(stream));

    public static SettingsTask Verify(
        Stream stream,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(stream, extension));

    public static SettingsTask Verify<T>(
        Task<T> stream,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
        where T : Stream =>
        Verify(settings, sourceFile, _ => _.VerifyStream(stream, extension));

    public static SettingsTask Verify<T>(
        IEnumerable<T?> streams,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
        where T : Stream =>
        Verify(settings, sourceFile, _ => _.VerifyStreams(streams, extension));
}