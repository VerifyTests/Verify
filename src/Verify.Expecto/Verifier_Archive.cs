// ReSharper disable RedundantSuppressNullableWarningExpression

using System.IO.Compression;

namespace VerifyExpecto;

public static partial class Verifier
{
    /// <summary>
    /// Verifies the contents of a <see cref="ZipArchive"/>.
    /// </summary>
    public static Task<VerifyResult> VerifyArchive(
        string name,
        ZipArchive archive,
        Func<ZipArchiveEntry, bool>? include = null,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? scrubber = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyArchive(archive, include, info, scrubber), true);
    }
}