namespace VerifyXunit;

public static partial class Verifier
{
    public static SettingsTask VerifyJson(
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyJson(target));

    public static SettingsTask VerifyJson(
        Stream? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyJson(target));
}