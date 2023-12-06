namespace VerifyExpecto;

// ReSharper disable RedundantSuppressNullableWarningExpression
public static partial class Verifier
{
    public static Task<VerifyResult> Verify(
        string name,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify());
    }

    public static Task<VerifyResult> Verify<T>(
        string name,
        Func<Task<T>> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(target()));
    }

    public static Task<VerifyResult> Verify<T>(
        string name,
        Task<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(target));
    }

    public static Task<VerifyResult> Verify<T>(
        string name,
        ValueTask<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(target));
    }

    public static Task<VerifyResult> Verify<T>(
        string name,
        IAsyncEnumerable<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(target));
    }

    public static Task<VerifyResult> Verify(
        string name,
        object? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(target));
    }
}