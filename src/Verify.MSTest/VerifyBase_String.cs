namespace VerifyMSTest;

partial class VerifyBase
{
#pragma warning disable CA1822 // Mark members as static

    [Pure]
    public SettingsTask Verify(
        [StringSyntax("*")]
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(target, settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask Verify(
        [StringSyntax("*")]
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(target, settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask Verify(
        [StringSyntax("*")]
        string? target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(target, extension, settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask Verify(
        [StringSyntax("*")]
        Task<string> target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(target, extension, settings, sourceFile, lineNumber);
}