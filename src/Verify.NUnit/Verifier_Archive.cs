using System.IO.Compression;

namespace VerifyNUnit;

public static partial class Verifier
{
    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive"/>
    /// </summary>
    public static SettingsTask Verify(
        ZipArchive archive,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyArchive(archive, include, info, fileScrubber), true);

    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive"/>
    /// </summary>
    public static SettingsTask VerifyArchive(
        string path,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyArchive(path, include, info, fileScrubber), true);

    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive"/>
    /// </summary>
    public static SettingsTask VerifyArchive(
        Stream stream,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyArchive(stream, include, info, fileScrubber), true);
}