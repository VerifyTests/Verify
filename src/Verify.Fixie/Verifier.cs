#pragma warning disable VerifySetParameters
namespace VerifyFixie;

public static partial class Verifier
{
    public static InnerVerifier BuildVerifier(string sourceFile, VerifySettings settings, bool useUniqueDirectory = false)
    {
        Guards.AgainstBadSourceFile(sourceFile);
        if (useUniqueDirectory)
        {
            settings.UseUniqueDirectory();
        }

        var state = ExecutionState.Current;

        if (!settings.HasParameters &&
            state.Parameters?.Length > 0)
        {
            settings.SetParameters(state.Parameters);
        }

        var method = state.Test.Method;
        var type = state.TestClass.Type;

        var pathInfo = GetPathInfo(sourceFile, type, method);
        return new(
            sourceFile,
            settings,
            type.NameWithParent(),
            method.Name,
            method.ParameterNames(),
            pathInfo);
    }

    static SettingsTask Verify(
        VerifySettings? settings,
        string sourceFile,
        Func<InnerVerifier, Task<VerifyResult>> verify,
        bool useUniqueDirectory = false)
    {
        Guards.AgainstBadSourceFile(sourceFile);
        return new(
            settings,
            async settings =>
            {
                using var verifier = BuildVerifier(sourceFile, settings, useUniqueDirectory);
                return await verify(verifier);
            });
    }

    [Pure]
    public static SettingsTask Verify(
        object? target,
        IEnumerable<Target> rawTargets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            sourceFile,
            _ => _.Verify(target, rawTargets));

    [Pure]
    public static SettingsTask Verify(
        IEnumerable<Target> targets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            sourceFile,
            _ => _.Verify(targets));

    [Pure]
    public static SettingsTask Verify(
        Target target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            sourceFile,
            _ => _.Verify(target));
}