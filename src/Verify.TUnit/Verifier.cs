#pragma warning disable VerifySetParameters
namespace VerifyTUnit;

public static partial class Verifier
{
    static Task AddFile(string path)
    {
        TestContext.Current?.Output.AttachArtifact(
            new()
            {
                File = new(path),
                Description = "Verify snapshot mismatch",
                DisplayName = Path.GetFileNameWithoutExtension(path)
            });
        return Task.CompletedTask;
    }

    [ModuleInitializer]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddAttachmentEvents() =>
        VerifierSettings.AddTestAttachment(AddFile);

    // TestContext.Current is null outside a test, for example in a
    // [Before(TestSession)] hook or in code that does not flow the test context.
    // Dereferencing it there gives a bare NullReferenceException.
    internal static TestDetails CurrentTestDetails()
    {
        var context = TestContext.Current;
        if (context is null)
        {
            throw new("TestContext.Current is null. Verify can only be used from within a test method.");
        }

        return context.Metadata.TestDetails;
    }

    public static InnerVerifier BuildVerifier(string sourceFile, VerifySettings settings, bool useUniqueDirectory = false, int lineNumber = 0)
    {
        Guards.AgainstBadSourceFile(sourceFile);
        if (useUniqueDirectory)
        {
            settings.UseUniqueDirectory();
        }

        var details = CurrentTestDetails();
        var type = details.MethodMetadata.Class.Type;
        var classArguments = details.TestClassArguments;
        var methodArguments = details.TestMethodArguments;
        var parameterNames = details.GetParameterNames();
        if (!settings.HasParameters &&
            (classArguments.Length > 0 ||
             methodArguments.Length > 0))
        {
            // Only apply when the argument count matches the parameter count. A params
            // array exposes raw pre-binding arguments, which TUnit bundles at invocation
            // time, so the counts differ and parameterized snapshot naming would throw.
            // MSTest and XunitV3 apply the same guard.
            if (classArguments.Length + methodArguments.Length == parameterNames?.Count)
            {
                settings.SetParameters([.. classArguments, .. methodArguments]);
                settings.SetClassArgumentCount(classArguments.Length);
            }
        }

        VerifierSettings.AssignTargetAssembly(type.Assembly);

        var method = details.MethodMetadata;
        var pathInfo = GetPathInfo(sourceFile, type, method.GetReflectionInfo());
        return new(
            sourceFile,
            settings,
            type.NameWithParent(),
            method.Name,
            parameterNames,
            pathInfo,
            lineNumber);
    }

    static SettingsTask Verify(
        VerifySettings? settings,
        string sourceFile,
        int lineNumber,
        Func<InnerVerifier, Task<VerifyResult>> verify,
        bool useUniqueDirectory = false)
    {
        Guards.AgainstBadSourceFile(sourceFile);
        return new(
            settings,
            async verifySettings =>
            {
                using var verifier = BuildVerifier(sourceFile, verifySettings, useUniqueDirectory, lineNumber);
                return await verify(verifier);
            });
    }

    [Pure]
    public static SettingsTask Verify(
        object? target,
        IEnumerable<Target> rawTargets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(
            settings,
            sourceFile,
            lineNumber,
            _ => _.Verify(target, rawTargets));

    [Pure]
    public static SettingsTask Verify(
        IEnumerable<Target> targets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(
            settings,
            sourceFile,
            lineNumber,
            _ => _.Verify(targets));

    [Pure]
    public static SettingsTask Verify(
        Target target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(
            settings,
            sourceFile,
            lineNumber,
            _ => _.Verify(target));
}