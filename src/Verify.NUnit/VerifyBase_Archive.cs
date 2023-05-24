using System.IO.Compression;

namespace VerifyNUnit;

public partial class VerifyBase
{
    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive"/>
    /// </summary>
    public SettingsTask Verify(
        ZipArchive archive,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null) =>
        Verifier.Verify(
            archive,
            include,
            settings ?? this.settings,
            info,
            fileScrubber,
            sourceFile);

    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive"/>
    /// </summary>
    public SettingsTask VerifyArchive(
        string path,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null) =>
        Verifier.VerifyArchive(
            path,
            include,
            settings ?? this.settings,
            info,
            fileScrubber,
            sourceFile);

    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive"/>
    /// </summary>
    public SettingsTask VerifyArchive(
        Stream stream,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null) =>
        Verifier.VerifyArchive(
            stream,
            include,
            settings ?? this.settings,
            info,
            fileScrubber,
            sourceFile);
}