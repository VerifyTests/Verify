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

        // Neither bare token must be content-searched: both are far too common in a file for the
        // search to land where it was aimed, and it only has to walk past a call another target
        // framework already accepted to splice a snapshot into someone else's test. They are
        // placeholders rather than content, so the patcher's insertion path replaces either
        if (expression is "null" or "default")
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
