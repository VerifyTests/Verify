namespace VerifyNUnit;

public partial class VerifyBase
{
    [Pure]
    public SettingsTask Verify(
        [StringSyntax("*")]
        string? target,
        VerifySettings? settings = null) =>
        Verifier.Verify(target, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask Verify(
        [StringSyntax("*")]
        Task<string> target,
        VerifySettings? settings = null) =>
        Verifier.Verify(target, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask Verify(
        [StringSyntax("*")]
        string? target,
        string extension,
        VerifySettings? settings = null) =>
        Verifier.Verify(target, extension, settings ?? this.settings, sourceFile);

    [Pure]
    public SettingsTask Verify(
        [StringSyntax("*")]
        Task<string> target,
        string extension,
        VerifySettings? settings = null) =>
        Verifier.Verify(target, extension, settings ?? this.settings, sourceFile);
}