namespace VerifyMSTest;

public partial class VerifyBase
{
    public SettingsTask Verify(
        byte[] bytes,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(bytes, extension));

    public SettingsTask Verify(
        Task<byte[]> bytes,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(bytes, extension));

    public SettingsTask Verify(
        FileStream stream,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(stream));

    public SettingsTask Verify(
        Task<FileStream> stream,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(stream));

    public SettingsTask Verify(
        Stream stream,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(stream, extension));

    public SettingsTask Verify<T>(
        Task<T> stream,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
        where T : Stream =>
        Verify(settings, sourceFile, _ => _.VerifyStream(stream, extension));

    public SettingsTask Verify<T>(
        IEnumerable<T> streams,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
        where T : Stream =>
        Verify(settings, sourceFile, _ => _.VerifyStreams(streams, extension));
}