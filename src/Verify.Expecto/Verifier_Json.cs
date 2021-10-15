using Newtonsoft.Json.Linq;
using VerifyTests;

namespace VerifyExpecto;

public static partial class Verifier
{
    public static Task Verify<T>(
        string name,
        Task<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(target));
    }

    public static Task Verify<T>(
        string name,
        ValueTask<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(target));
    }

    public static Task Verify<T>(
        string name,
        IAsyncEnumerable<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(target));
    }

    public static Task Verify<T>(
        string name,
        T target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(target));
    }

    public static Task VerifyJson(
        string name,
        string target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyJson(target));
    }

    public static Task VerifyJson(
        string name,
        JToken target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyJson(target));
    }

    public static Task VerifyJson(
        string name,
        Stream target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyJson(target));
    }
}