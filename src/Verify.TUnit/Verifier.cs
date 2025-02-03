#pragma warning disable VerifySetParameters
namespace VerifyTUnit;

public static partial class Verifier
{
    static Task AddFile(FilePair path)
    {
        TestContext.Current!.AddArtifact(
            new()
            {
                File = new(path.ReceivedPath),
                Description = "Verify snapshot mismatch",
                DisplayName = Path.GetFileNameWithoutExtension(path.ReceivedPath)
            });
        return Task.CompletedTask;
    }

    static Verifier()
    {
        VerifierSettings.OnFirstVerify((pair, _, _) => AddFile(pair));
        VerifierSettings.OnVerifyMismatch((pair, _, _) => AddFile(pair));
    }

    static InnerVerifier BuildVerifier(string sourceFile, VerifySettings settings, bool useUniqueDirectory)
    {
        Guards.AgainstBadSourceFile(sourceFile);
        if (useUniqueDirectory)
        {
            settings.UseUniqueDirectory();
        }

        var details = TestContext.Current!.TestDetails;
        var type = details.TestClass.Type;
        var classArguments = details.TestClassArguments;
        var methodArguments = details.TestMethodArguments;
        if (!settings.HasParameters &&
            (classArguments.Length > 0 ||
             methodArguments.Length > 0))
        {
            settings.SetParameters([.. classArguments, .. methodArguments]);
        }

        VerifierSettings.AssignTargetAssembly(type.Assembly);

        var method = details.TestMethod;
        var pathInfo = GetPathInfo(sourceFile, type, method.ReflectionInformation);
        return new(
            sourceFile,
            settings,
            type.NameWithParent(),
            method.Name,
            details.GetParameterNames(),
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
            async verifySettings =>
            {
                using var verifier = BuildVerifier(sourceFile, verifySettings, useUniqueDirectory);
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