namespace VerifyXunit;

public static partial class Verifier
{
    public static SettingsTask VerifyXml(
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyXml(target));

    public static SettingsTask VerifyXml(
        Stream? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyXml(target));
}