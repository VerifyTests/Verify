namespace VerifyXunit;

public static partial class Verifier
{
    static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile, bool useUniqueDirectory)
    {
        if (!UsesVerifyAttribute.TryGet(out var method))
        {
            var fileName = Path.GetFileName(sourceFile);
            throw new($"Expected to find a `[UsesVerify]` on test class. File: {fileName}.");
        }

        if (useUniqueDirectory)
        {
            settings.UseUniqueDirectory();
        }

        var type = method.ReflectedType!;
        TargetAssembly.Assign(type.Assembly);

        var methodParameters = method.ParameterNames();

        var pathInfo = GetPathInfo(sourceFile, type, method);
        return new(
            sourceFile,
            settings,
            type.NameWithParent(),
            method.Name,
            methodParameters,
            pathInfo);
    }

    public static SettingsTask Verify(
        object? target,
        IEnumerable<Target> rawTargets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            IoHelpers.GetMappedBuildPath(sourceFile),
            _ => _.Verify(target, rawTargets));

    public static SettingsTask Verify(
        IEnumerable<Target> targets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            IoHelpers.GetMappedBuildPath(sourceFile),
            _ => _.Verify(targets));

    static SettingsTask Verify(
        VerifySettings? settings,
        string sourceFile,
        Func<InnerVerifier, Task<VerifyResult>> verify,
        bool useUniqueDirectory = false)
    {
        Guard.AgainstBadSourceFile(sourceFile);
        return new(
            settings,
            async verifySettings =>
            {
                using var verifier = GetVerifier(verifySettings, sourceFile, useUniqueDirectory);
                return await verify(verifier);
            });
    }
}