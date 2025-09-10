namespace VerifyMSTest;

partial class VerifyBase
{
#pragma warning disable CA1822 // Mark members as static

    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive" />
    /// </summary>
    [Pure]
    public SettingsTask Verify(
        ZipArchive archive,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        bool includeStructure = false,
        bool persistArchive = false,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Verify(archive, include, settings, info, fileScrubber, includeStructure, persistArchive, sourceFile);

    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive" />
    /// </summary>
    [Pure]
    public SettingsTask VerifyZip(
        string path,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        bool includeStructure = false,
        bool persistArchive = false,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyZip(path, include, settings, info, fileScrubber, includeStructure, persistArchive, sourceFile);

    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive" />
    /// </summary>
    [Pure]
    public SettingsTask VerifyZip(
        Stream stream,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        bool includeStructure = false,
        bool persistArchive = false,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyZip(stream, include, settings, info, fileScrubber, includeStructure, persistArchive, sourceFile);

    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive" />
    /// </summary>
    [Pure]
    public SettingsTask VerifyZip(
        byte[] bytes,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        bool includeStructure = false,
        bool persistArchive = false,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.VerifyZip(bytes, include, settings, info, fileScrubber, includeStructure, persistArchive, sourceFile);
}