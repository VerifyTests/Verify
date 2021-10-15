using Newtonsoft.Json.Linq;
using VerifyTests;

namespace VerifyMSTest;

public partial class VerifyBase
{
    public SettingsTask Verify<T>(
        Task<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    public SettingsTask Verify<T>(
        ValueTask<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    public SettingsTask Verify<T>(
        IAsyncEnumerable<T> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    public SettingsTask Verify<T>(
        T target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    public SettingsTask VerifyJson(
        string target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.VerifyJson(target));
    }

    public SettingsTask VerifyJson(
        JToken target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.VerifyJson(target));
    }

    public SettingsTask VerifyJson(
        Stream target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.VerifyJson(target));
    }
}