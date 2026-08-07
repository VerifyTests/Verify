namespace VerifyNUnit;

public partial class VerifyBase
{
    [Pure]
    public SettingsTask VerifyInline(
        [StringSyntax("*")]
        string? target,
        [StringSyntax("*")]
        string? expected = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerArgumentExpression(nameof(expected))] string? expression = null) =>
        Verifier.VerifyInline(target, expected, settings ?? this.settings, sourceFile, lineNumber, expression);

    [Pure]
    public SettingsTask VerifyInline(
        object? target,
        [StringSyntax("*")]
        string? expected = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerArgumentExpression(nameof(expected))] string? expression = null) =>
        Verifier.VerifyInline(target, expected, settings ?? this.settings, sourceFile, lineNumber, expression);
}
