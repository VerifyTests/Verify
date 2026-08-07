namespace VerifyTests;

// Captured at the VerifyInline call site. File/Line/Expression drive source rewriting on accept.
sealed class InlineInfo(string? expected, string file, int line, string? expression)
{
    // Null means: new snapshot, populate the literal on accept
    public string? Expected { get; } = expected;

    // CallerFilePath of the VerifyInline call
    public string File { get; } = file;

    // CallerLineNumber of the VerifyInline call. A hint only; content search is the locator
    public int Line { get; } = line;

    // CallerArgumentExpression of `expected` (the literal source text, including quotes).
    // Null when no expected argument was passed, or the argument was a bare null
    public string? Expression { get; } = expression;
}
