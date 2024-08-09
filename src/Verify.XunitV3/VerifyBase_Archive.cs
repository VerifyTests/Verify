using System.IO.Compression;

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
        FileScrubber? fileScrubber = null) =>
        Verifier.Verify(
            archive,
            include,
            settings ?? this.settings,
            info,
            fileScrubber,
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
        FileScrubber? fileScrubber = null) =>
        Verifier.VerifyZip(
            path,
            include,
            settings ?? this.settings,
            info,
            fileScrubber,
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
        FileScrubber? fileScrubber = null) =>
        Verifier.VerifyZip(
            stream,
            include,
            settings ?? this.settings,
            info,
            fileScrubber,
            sourceFile);
}