using Polyfills;

namespace VerifyXunit;

public static partial class Verifier
{
    /// <summary>
    /// Verifies the contents of <paramref name="path" />.
    /// </summary>
    [Pure]
    public static SettingsTask VerifyFile(
        string path,
        VerifySettings? settings = null,
        object? info = null,
        string? extension = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyFile(path, info, extension));

    /// <summary>
    /// Verifies the contents of files.
    /// </summary>
    [Pure]
    public static SettingsTask VerifyFiles(
        ReadOnlySpan<string> paths,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
    {
        Guard.NotEmpty(paths);
        var path = paths[0];
        var task = Verify(settings, sourceFile, _ => _.VerifyFile(path, info));
        for (var index = 1; index < paths.Length; index++)
        {
            var item = paths[index];
            task = task.AppendFile(item);
        }

        return task;
    }

    /// <summary>
    /// Verifies the contents of <paramref name="path" />.
    /// Differs from passing <see cref="FileInfo" /> to <code>Verify(object? target)</code> which will verify the full path.
    /// </summary>
    [Pure]
    public static SettingsTask VerifyFile(
        FileInfo path,
        VerifySettings? settings = null,
        object? info = null,
        string? extension = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyFile(path, info, extension));
}