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

        var context = GetTestContext();

        var assembly = context.Assembly;
        var type = context.TestClass;
        var method = context.Method;

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

    internal static TestExecutionContext GetTestContext()
    {
        var context = CurrentTestContext.Value;
        if (context is null)
        {
            throw new($"TestContext is null. {AttributeUsageHelp}");
        }

        return context;
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
        Guards.AgainstBadSourceFile(sourceFile);
        return new(
            settings,
            async settings =>
            {
                using var verifier = BuildVerifier(settings, sourceFile, useUniqueDirectory);
                return await verify(verifier);
            });
    }
}