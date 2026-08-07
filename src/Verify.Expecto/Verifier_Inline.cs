namespace VerifyExpecto;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask VerifyInline(
        string name,
        [StringSyntax("*")]
        string? target,
        [StringSyntax("*")]
        string? expected = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerArgumentExpression(nameof(expected))] string? expression = null)
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(InlineSettings(expected, settings, sourceFile, lineNumber, expression), assembly, sourceFile, name, _ => _.VerifyString(target));
    }

    [Pure]
    public static SettingsTask VerifyInline(
        string name,
        object? target,
        [StringSyntax("*")]
        string? expected = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerArgumentExpression(nameof(expected))] string? expression = null)
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(InlineSettings(expected, settings, sourceFile, lineNumber, expression), assembly, sourceFile, name, _ => _.Verify(target, []));
    }

    static VerifySettings InlineSettings(string? expected, VerifySettings? settings, string file, int line, string? expression)
    {
        var inline = new VerifySettings(settings);
        inline.Inline(expected, file, line, expression);
        return inline;
    }
}
