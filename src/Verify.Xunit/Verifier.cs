namespace VerifyXunit;

public static partial class Verifier
{
    static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile)
    {
        if (!UsesVerifyAttribute.TryGet(out var method))
        {
            var fileName = Path.GetFileName(sourceFile);
            throw new($"Expected to find a `[UsesVerify]` on test class. File: {fileName}.");
        }

        var type = method.ReflectedType!;
        TargetAssembly.Assign(type.Assembly);

        var methodParameters = method.ParameterNames();
        var typeName = type.NameWithParent();
        var methodName = method.Name;

        return new(
            sourceFile,
            settings,
            typeName,
            methodName,
            methodParameters);
    }

    public static SettingsTask Verify(
        object? target,
        IEnumerable<Target> targets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            sourceFile,
            _ => _.Verify(target, null, targets));

    static SettingsTask Verify(VerifySettings? settings, string sourceFile, Func<InnerVerifier, Task<VerifyResult>> verify)
    {
        Guard.AgainstBadSourceFile(sourceFile);
        return new(
            settings,
            async verifySettings =>
            {
                using var verifier = GetVerifier(verifySettings, sourceFile);
                return await verify(verifier);
            });
    }
}