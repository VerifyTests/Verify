namespace VerifyTests.ExceptionParsing;

public readonly struct InlineEntry(
    string sourceFile,
    int line,
    string? receivedPath,
    string? expectedPath,
    string? patchPath)
{
    /// <summary>
    /// Full path to the .cs file containing the inline snapshot.
    /// </summary>
    public string SourceFile { get; } = sourceFile;

    /// <summary>
    /// 1 based line of the VerifyInline call.
    /// </summary>
    public int Line { get; } = line;

    /// <summary>
    /// Staged received text file (absolute). Null when staging was unavailable.
    /// </summary>
    public string? ReceivedPath { get; } = receivedPath;

    /// <summary>
    /// Staged expected text file (absolute). Null when staging was unavailable.
    /// </summary>
    public string? ExpectedPath { get; } = expectedPath;

    /// <summary>
    /// Staged patch file (absolute), readable via DiffEngine InlinePatchFile. Null when staging was unavailable.
    /// </summary>
    public string? PatchPath { get; } = patchPath;
}
