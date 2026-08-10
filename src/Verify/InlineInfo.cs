namespace VerifyTests;

// Where the inline snapshot lives and how the source is rewritten on accept.
sealed class InlineInfo(string? expected, string file, int line, string? expression, InlinePatchMode mode)
{
    // Null means: new snapshot, populate the literal on accept
    public string? Expected { get; } = expected;

    // CallerFilePath of the Snapshot call, or of the verify call when there is not one yet
    public string File { get; } = file;

    // CallerLineNumber of that same call. A hint only; content search is the locator
    public int Line { get; } = line;

    // CallerArgumentExpression of `expected` (the literal source text, including quotes).
    // Null when no expected argument was passed, or the argument was a bare null
    public string? Expression { get; } = expression;

    // Set when there is a Snapshot call to put the literal in, Append when there is not
    public InlinePatchMode Mode { get; } = mode;
}
