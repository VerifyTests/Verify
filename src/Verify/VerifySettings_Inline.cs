namespace VerifyTests;

public partial class VerifySettings
{
    internal InlineInfo? inline;

    /// <summary>
    /// Compare the result against an inline snapshot instead of a .verified file.
    /// Passing no <paramref name="expected" /> means: new snapshot, populate the literal on accept.
    /// Only supported from C# source files, and only for text results.
    /// </summary>
    public void Inline(
        [StringSyntax("*")] string? expected = null,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerArgumentExpression(nameof(expected))] string? expression = null)
    {
        Guards.AgainstBadSourceFile(file);
        if (!file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
        {
            throw new(
                $"""
                 Inline snapshots are only supported from C# source files.
                 SourceFile: {file}
                 """);
        }

        // A bare `null` token must not be content-searched (far too common); the
        // patcher's insertion path handles replacing it
        if (expression == "null")
        {
            expression = null;
        }

        inline = new(expected, file, line, expression);
    }
}
