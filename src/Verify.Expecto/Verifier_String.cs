// ReSharper disable RedundantSuppressNullableWarningExpression

namespace VerifyExpecto;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask Verify(
        string name,
        [StringSyntax("*")]
        string? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, lineNumber, name, _ => _.VerifyString(target));
    }

    [Pure]
    public static SettingsTask Verify(
        string name,
        [StringSyntax("*")]
        Task<string> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, lineNumber, name, _ => _.VerifyString(target));
    }

    [Pure]
    public static SettingsTask Verify(
        string name,
        [StringSyntax("*")]
        string? target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, lineNumber, name, _ => _.VerifyString(target, extension));
    }

    [Pure]
    public static SettingsTask Verify(
        string name,
        [StringSyntax("*")]
        Task<string> target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, lineNumber, name, _ => _.VerifyString(target, extension));
    }
}