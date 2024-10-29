// ReSharper disable RedundantSuppressNullableWarningExpression

namespace VerifyExpecto;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask Throws(
        string name,
        Action target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.Throws(target));
    }

    [Pure]
    public static SettingsTask Throws(
        string name,
        Func<object?> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.Throws(target));
    }

    [Pure]
    public static SettingsTask ThrowsTask(
        string name,
        Func<Task> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.ThrowsTask(target));
    }

    [Pure]
    public static SettingsTask ThrowsTask<T>(
        string name,
        Func<Task<T>> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.ThrowsTask(target));
    }

    [Pure]
    public static SettingsTask ThrowsValueTask(
        string name,
        Func<ValueTask> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.ThrowsValueTask(target));
    }

    [Pure]
    public static SettingsTask ThrowsValueTask<T>(
        string name,
        Func<ValueTask<T>> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.ThrowsValueTask(target));
    }
}