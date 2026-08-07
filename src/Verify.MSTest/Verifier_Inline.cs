namespace VerifyMSTest;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask VerifyInline(
        [StringSyntax("*")]
        string? target,
        [StringSyntax("*")]
        string? expected = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerArgumentExpression(nameof(expected))] string? expression = null) =>
        Verify(target, InlineSettings(expected, settings, sourceFile, lineNumber, expression), sourceFile);

    [Pure]
    public static SettingsTask VerifyInline(
        object? target,
        [StringSyntax("*")]
        string? expected = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerArgumentExpression(nameof(expected))] string? expression = null) =>
        Verify(target, InlineSettings(expected, settings, sourceFile, lineNumber, expression), sourceFile);

    static VerifySettings InlineSettings(string? expected, VerifySettings? settings, string file, int line, string? expression)
    {
        var inline = new VerifySettings(settings);
        inline.Inline(expected, file, line, expression);
        return inline;
    }
}
