namespace VerifyXunit;

public partial class VerifyBase
{
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
        bool includeStructure = false) =>
        Verifier.Verify(
            archive,
            include,
            settings ?? this.settings,
            info,
            fileScrubber,
            includeStructure,
            sourceFile);

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
        bool includeStructure = false) =>
        Verifier.VerifyZip(
            path,
            include,
            settings ?? this.settings,
            info,
            fileScrubber,
            includeStructure,
            sourceFile);

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
        bool includeStructure = false) =>
        Verifier.VerifyZip(
            stream,
            include,
            settings ?? this.settings,
            info,
            fileScrubber,
            includeStructure,
            sourceFile);

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
        bool includeStructure = false) =>
        Verifier.VerifyZip(
            bytes,
            include,
            settings ?? this.settings,
            info,
            fileScrubber,
            includeStructure,
            sourceFile);
}