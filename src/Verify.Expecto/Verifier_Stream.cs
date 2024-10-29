// ReSharper disable RedundantSuppressNullableWarningExpression

namespace VerifyExpecto;

public static partial class Verifier
{
    [Pure]
    public static SettingsTask Verify(
        string name,
        byte[]? target,
        string extension,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyStream(target, extension, info));
    }

    [Pure]
    public static SettingsTask Verify(
        string name,
        byte[]? target,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyStream(target, info));
    }

    [Pure]
    public static SettingsTask Verify(
        string name,
        Task<byte[]> target,
        string extension,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyStream(target, extension, info));
    }

    [Pure]
    public static SettingsTask Verify(
        string name,
        ValueTask<byte[]> target,
        string extension,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyStream(target, extension, info));
    }

    [Pure]
    public static SettingsTask Verify(
        string name,
        FileStream? target,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyStream(target, info));
    }

    [Pure]
    public static SettingsTask Verify(
        string name,
        Stream? target,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyStream(target, info));
    }

    [Pure]
    public static SettingsTask Verify(
        string name,
        Stream? target,
        string extension,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyStream(target, extension, info));
    }

    [Pure]
    public static SettingsTask Verify<T>(
        string name,
        Task<T> target,
        string extension,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
        where T : Stream
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyStream(target, extension, info));
    }

    [Pure]
    public static SettingsTask Verify<T>(
        string name,
        ValueTask<T> target,
        string extension,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
        where T : Stream
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyStream(target, extension, info));
    }

    [Pure]
    public static SettingsTask Verify<T>(
        string name,
        IEnumerable<T> targets,
        string extension,
        VerifySettings? settings = null,
        object? info = null,
        [CallerFilePath] string sourceFile = "")
        where T : Stream
    {
        var assembly = Assembly.GetCallingAssembly()!;
        return Verify(settings, assembly, sourceFile, name, _ => _.VerifyStreams(targets, extension, info));
    }
}