using System.IO.Compression;

namespace VerifyNUnit;

public partial class VerifyBase
{
    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive"/>.
    /// </summary>
    public SettingsTask VerifyArchive(
        ZipArchive archive,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.VerifyArchive(archive, include, settings ?? this.settings, sourceFile);
}