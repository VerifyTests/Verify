namespace VerifyExpecto;

public static partial class Verifier
{
    public static InnerVerifier BuildVerifier(VerifySettings settings, string sourceFile, string methodName, bool useUniqueDirectory = false)
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

    static SettingsTask Verify(
        VerifySettings? settings,
        Assembly assembly,
        string sourceFile,
        string name,
        Func<InnerVerifier, Task<VerifyResult>> verify,
        bool useUniqueDirectory = false)
    {
        VerifierSettings.AssignTargetAssembly(assembly);
        settings ??= new();
        Guards.AgainstBadSourceFile(sourceFile);
        return new(
            settings,
            async settings =>
            {
                using var verifier = BuildVerifier(settings, sourceFile, name, useUniqueDirectory);

                //TODO: rest and replicate try in other projects
                try
                {
                    return await verify(verifier);
                }
                catch (TargetInvocationException exception)
                    when (exception.InnerException != null)
                {
                    throw exception.InnerException!;
                }
            });
    }

    public static SettingsTask Verify(
        string name,
        object? target,
        IEnumerable<Target> rawTargets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly();
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(target, rawTargets));
    }

    public static SettingsTask Verify(
        string name,
        IEnumerable<Target> targets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly();
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(targets));
    }

    public static SettingsTask Verify(
        string name,
        Target target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var assembly = Assembly.GetCallingAssembly();
        return Verify(settings, assembly, sourceFile, name, _ => _.Verify(target));
    }
}