namespace VerifyExpecto;

// ReSharper disable RedundantSuppressNullableWarningExpression

public static partial class Verifier
{
    public static Task<VerifyResult> VerifyXml(
        string name,
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyXml(target));
    }

    public static Task<VerifyResult> VerifyXml(
        string name,
        Stream? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyXml(target));
    }
}