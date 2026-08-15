namespace VerifyTests;

// Where the inline snapshot lives and how the source is rewritten on accept.
sealed class InlineInfo(string? expected, string file, int line, string? expression, string? member, InlinePatchMode mode)
{
    // Null means: new snapshot, populate the literal on accept
    public string? Expected { get; } = expected;

    // CallerFilePath of the Snapshot call, or of the verify call when there is not one yet
    public string File { get; } = file;

    // CallerLineNumber of that same call. A hint only; content search is the locator
    public int Line { get; } = line;

    // CallerArgumentExpression of `expected` (the literal source text, including quotes).
    // Null when no expected argument was passed, or the argument was a bare null.
    // Always null from F#, whose compiler does not implement the attribute (FS0202), which is why
    // the patch also carries the previous value
    public string? Expression { get; } = expression;

    // CallerMemberName of the call, which every language supplies. Narrows the search for the call
    // site to the member the patch came from
    public string? MemberName { get; } = string.IsNullOrEmpty(member) ? null : member;

    // Set when there is a Snapshot call to put the literal in, Append when there is not
    public InlinePatchMode Mode { get; } = mode;

    // The language the literal is written in, which decides how a value is written and read back
    public SourceLanguage Language { get; } = SourceLanguage.ForFile(file);

    /// <summary>
    /// The languages DiffEngine can rewrite a literal in. Checked before a snapshot is inlined,
    /// because a file it cannot patch produces a snapshot that never applies and a test that fails
    /// forever.
    /// </summary>
    public static bool IsSupported(string file) =>
        file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) ||
        file.EndsWith(".fs", StringComparison.OrdinalIgnoreCase) ||
        file.EndsWith(".fsx", StringComparison.OrdinalIgnoreCase);
}
