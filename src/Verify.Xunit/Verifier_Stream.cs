namespace VerifyXunit;

public static partial class Verifier
{
    public static SettingsTask Verify(
        FileStream target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target));

    public static SettingsTask Verify(
        Task<FileStream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target));

    public static SettingsTask Verify(
        Stream target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, extension));

    public static SettingsTask Verify<T>(
        Task<T> target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
        where T : Stream  =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, extension));

    public static SettingsTask Verify<T>(
        IEnumerable<T> targets,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
        where T : Stream  =>
        Verify(settings, sourceFile, _ => _.VerifyStreams(targets, extension));

    public static SettingsTask Verify(
        byte[] target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, extension));

    public static SettingsTask Verify(
        Task<byte[]> target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, extension));
}