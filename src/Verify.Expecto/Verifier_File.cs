// ReSharper disable RedundantSuppressNullableWarningExpression

namespace VerifyExpecto;

public static partial class Verifier
{
    /// <summary>
    /// Verifies the contents of <paramref name="path" />.
    /// </summary>
    [Pure]
    public static SettingsTask VerifyFile(
        string name,
        string path,
        VerifySettings? settings = null,
        object? info = null,
        string? extension = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyFile(path, info, extension));
    }

    /// <summary>
    /// Verifies the contents of files.
    /// </summary>
    [Pure]
    public static SettingsTask VerifyFiles(
        string name,
        IEnumerable<string> paths,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyFiles(paths, info, fileScrubber));
    }

    /// <summary>
    /// Verifies the contents of <paramref name="path" />.
    /// Differs from passing <see cref="FileInfo" /> to <code>Verify(object? target)</code> which will verify the full path.
    /// </summary>
    [Pure]
    public static SettingsTask VerifyFile(
        string name,
        FileInfo path,
        VerifySettings? settings = null,
        object? info = null,
        string? extension = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyFile(path, info, extension));
    }
}