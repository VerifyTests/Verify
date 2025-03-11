namespace VerifyTUnit;

public static partial class Verifier
{
    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive" />
    /// </summary>
    [Pure]
    public static SettingsTask Verify(
        ZipArchive archive,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        bool includeStructure = false,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyZip(archive, include, info, fileScrubber, includeStructure), true);

    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive" />
    /// </summary>
    [Pure]
    public static SettingsTask VerifyZip(
        string path,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        bool includeStructure = false,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyZip(path, include, info, fileScrubber, includeStructure), true);

    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive" />
    /// </summary>
    [Pure]
    public static SettingsTask VerifyZip(
        Stream stream,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        bool includeStructure = false,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyZip(stream, include, info, fileScrubber, includeStructure), true);

    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive" />
    /// </summary>
    [Pure]
    public static SettingsTask VerifyZip(
        byte[] bytes,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        bool includeStructure = false,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyZip(bytes, include, info, fileScrubber, includeStructure), true);
}