namespace VerifyNUnit;

public partial class VerifyBase
{
    [Pure]
    public SettingsTask Verify(
        [StringSyntax("*")]
        string? target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask Verify(
        [StringSyntax("*")]
        Task<string> target,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(target, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask Verify(
        [StringSyntax("*")]
        string? target,
        string extension,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(target, extension, settings ?? this.settings, sourceFile, lineNumber);

    [Pure]
    public SettingsTask Verify(
        [StringSyntax("*")]
        Task<string> target,
        string extension,
        VerifySettings? settings = null,
        [CallerLineNumber] int lineNumber = 0) =>
        Verifier.Verify(target, extension, settings ?? this.settings, sourceFile, lineNumber);
}