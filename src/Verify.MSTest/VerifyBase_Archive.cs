using System.IO.Compression;

namespace VerifyMSTest;

public partial class VerifyBase
{
    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive"/>.
    /// </summary>
    public SettingsTask VerifyArchive(
        ZipArchive archive,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? scrubber = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyArchive(archive, include, info, scrubber), true);
}