using System.IO.Compression;

namespace VerifyNUnit;

public static partial class Verifier
{
    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive"/>.
    /// </summary>
    public static SettingsTask VerifyArchive(
        ZipArchive archive,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? scrubber = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyArchive(archive, include, info, scrubber), true);
}