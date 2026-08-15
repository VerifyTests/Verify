namespace VerifyTests;

public partial class VerifySettings
{
    internal InlineInfo? inline;
    internal bool notInline;

    /// <summary>
    /// Compare the result against an inline snapshot instead of a <c>.verified</c> file.
    /// Passing no <paramref name="expected" /> means: new snapshot, populate the literal on accept.
    /// Supported from C# and F# source files, and only for text results.
    /// <para>
    /// Calling this is an explicit opt in, so it applies whether or not
    /// <see cref="VerifierSettings.Inline" /> has been used. When inline is off for this
    /// verification, the call is removed from the source and the snapshot moves to a file.
    /// </para>
    /// </summary>
    public void Snapshot(
        [StringSyntax("*")][ConstantExpected]  string? expected = null,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerArgumentExpression(nameof(expected))] string? expression = null,
        [CallerMemberName] string member = "")
    {
        Guards.AgainstBadSourceFile(file);
        if (!InlineInfo.IsSupported(file))
        {
            throw new(
                $"""
                 Inline snapshots are only supported from C# and F# source files.
                 SourceFile: {file}
                 """);
        }

        // A bare `null` token must not be content-searched (far too common); the
        // patcher's insertion path handles replacing it
        if (expression == "null")
        {
            expression = null;
        }

        inline = new(expected, file, line, expression, member, InlinePatchMode.Set);
    }

    /// <summary>
    /// Use a <c>.verified</c> file for this verification even when
    /// <see cref="VerifierSettings.Inline" /> is on.
    /// </summary>
    public void NotInline() =>
        notInline = true;
}
