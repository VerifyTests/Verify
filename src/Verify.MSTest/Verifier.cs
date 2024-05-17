namespace VerifyMSTest;

public static partial class Verifier
{
    const string AttributeUsageHelp = "Ensure test class has a `[UsesVerify]` attribute (or inherits from `VerifyBase`).";

    static Task AddFile(FilePair path, bool autoVerify)
    {
        var context = CurrentTestContext.Value?.TestContext;
        if (context != null)
        {
            var fileName = autoVerify ? path.VerifiedPath : path.ReceivedPath;
            context.AddResultFile(fileName);
        }

        return Task.CompletedTask;
    }

    static Verifier()
    {
        VerifierSettings.OnFirstVerify((pair, _, autoVerify) => AddFile(pair, autoVerify));
        VerifierSettings.OnVerifyMismatch((pair, _, autoVerify) => AddFile(pair, autoVerify));
    }

    public static readonly AsyncLocal<TestExecutionContext?> CurrentTestContext = new();

    static InnerVerifier BuildVerifier(VerifySettings settings, string sourceFile, bool useUniqueDirectory)
    {
        if (useUniqueDirectory)
        {
            settings.UseUniqueDirectory();
        }

        if (CurrentTestContext.Value is null)
        {
            throw new($"TestContext is null. {AttributeUsageHelp}");
        }

        if (CurrentTestContext.Value?.TestContext is null)
        {
            throw new($"TestContext.TestContext is null. {AttributeUsageHelp}");
        }

        if (CurrentTestContext.Value?.TestContext.TestName is null)
        {
            throw new($"TestContext.TestContext.TestName is null. {AttributeUsageHelp}");
        }

        if (CurrentTestContext.Value?.TestContext.FullyQualifiedTestClassName is null)
        {
            throw new($"TestContext.TestContext.FullyQualifiedTestClassName is null. {AttributeUsageHelp}");
        }

        (var assembly, var type, var method) = TestContextReflector.Get(CurrentTestContext.Value.TestContext);
        VerifierSettings.AssignTargetAssembly(assembly);
        var pathInfo = GetPathInfo(sourceFile, type, method);
        return new(
            sourceFile,
            settings,
            type.NameWithParent(),
            method.Name,
            method.ParameterNames(),
            pathInfo);
    }

    [Pure]
    public static SettingsTask Verify(
        object? target,
        IEnumerable<Target> rawTargets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.Verify(target, rawTargets));

    [Pure]
    public static SettingsTask Verify(
        IEnumerable<Target> targets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.Verify(targets));

    [Pure]
    public static SettingsTask Verify(
        Target target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.Verify(target));

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
                using var verifier = BuildVerifier(verifySettings, sourceFile, useUniqueDirectory);
                return await verify(verifier);
            });
    }
}