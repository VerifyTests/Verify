namespace VerifyXunit;

public static partial class Verifier
{
    public static SettingsTask Verify<T>(
        Task<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    public static SettingsTask Verify<T>(
        ValueTask<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    public static SettingsTask Verify<T>(
        IAsyncEnumerable<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    public static SettingsTask Verify<T>(
        T target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    public static SettingsTask VerifyJson(
        string target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.VerifyJson(target));
    }

    public static SettingsTask VerifyJson(
        JToken target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.VerifyJson(target));
    }

    public static SettingsTask VerifyJson(
        Stream target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.VerifyJson(target));
    }
}