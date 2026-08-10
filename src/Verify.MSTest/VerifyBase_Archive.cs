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
        string? archiveExtension = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(archive, include, settings, info, fileScrubber, includeStructure, persistArchive, archiveExtension, sourceFile, lineNumber);

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
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.VerifyZip(path, include, settings, info, fileScrubber, includeStructure, persistArchive, sourceFile, lineNumber);

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
        string? archiveExtension = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.VerifyZip(stream, include, settings, info, fileScrubber, includeStructure, persistArchive, archiveExtension, sourceFile, lineNumber);

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
        string? archiveExtension = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.VerifyZip(bytes, include, settings, info, fileScrubber, includeStructure, persistArchive, archiveExtension, sourceFile, lineNumber);
}