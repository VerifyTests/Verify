﻿namespace VerifyNUnit;

public partial class VerifyBase
{
    /// <summary>
    /// Verifies the contents of <paramref name="path" />.
    /// </summary>
    [Pure]
    public SettingsTask VerifyFile(
        string path,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.VerifyFile(path, settings ?? this.settings, info, sourceFile);

    /// <summary>
    /// Verifies the contents of files.
    /// </summary>
    [Pure]
    public SettingsTask VerifyFiles(
        IEnumerable<string> paths,
        VerifySettings? settings = null,
        object? info = null,
        FileScrubber? fileScrubber = null) =>
        Verifier.VerifyFiles(paths, settings ?? this.settings, info, fileScrubber, sourceFile);

    /// <summary>
    /// Verifies the contents of <paramref name="path" />.
    /// Differs from passing <see cref="FileInfo" /> to <code>Verify(object? target)</code> which will verify the full path.
    /// </summary>
    [Pure]
    public SettingsTask VerifyFile(
        FileInfo path,
        VerifySettings? settings = null,
        object? info = null) =>
        Verifier.VerifyFile(path, settings ?? this.settings, info, sourceFile);
}