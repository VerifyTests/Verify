namespace VerifyExpecto;

public static partial class Verifier
{
    static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile, string methodName, bool useUniqueDirectory)
    {
        if (settings.HasParameters)
        {
            ThrowNotSupported(nameof(VerifySettings.UseParameters));
        }

        if (settings.parametersText is not null)
        {
            ThrowNotSupported(nameof(VerifySettings.UseTextForParameters));
        }

        if (useUniqueDirectory)
        {
            settings.UseUniqueDirectory();
        }

        sourceFile = IoHelpers.GetMappedBuildPath(sourceFile);
        var fileName = Path.GetFileNameWithoutExtension(sourceFile);

        var pathInfo = GetPathInfo(sourceFile, fileName, methodName);
        return new(sourceFile, settings, fileName, methodName, null, pathInfo);
    }

    [DoesNotReturn]
    static void ThrowNotSupported(string api) =>
        throw new($"Expect does not support `{api}()`. Change the `name` parameter instead.");

    static async Task<VerifyResult> Verify(
        VerifySettings? settings,
        Assembly assembly,
        string sourceFile,
        string name,
        Func<InnerVerifier, Task<VerifyResult>> verify,
        bool useUniqueDirectory = false)
    {
        VerifierSettings.AssignTargetAssembly(assembly);
        settings ??= new();
        Guard.AgainstBadSourceFile(sourceFile);
        using var verifier = GetVerifier(settings, sourceFile, name, useUniqueDirectory);
        return await verify(verifier);
    }

    public static Task<VerifyResult> Verify(
        string name,
        object? target,
        IEnumerable<Target> rawTargets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly();
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(target, rawTargets));
    }

    public static Task<VerifyResult> Verify(
        string name,
        IEnumerable<Target> targets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly();
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(targets));
    }

    public static Task<VerifyResult> Verify(
        string name,
        Target target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly();
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(target));
    }
}