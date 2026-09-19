#pragma warning disable VerifySetParameters
namespace VerifyMSTest;

public static partial class Verifier
{
    static Task AddFile(string path)
    {
        // Attaching only needs the TestContext, so this takes the ambient fallback without
        // resolving the test class.
        var context = CurrentTestContext.Value?.TestContext ?? AmbientContext;
        context?.AddResultFile(path);
        return Task.CompletedTask;
    }

#pragma warning disable MSTESTEXP
    static TestContext? AmbientContext => TestContext.Current;
#pragma warning restore MSTESTEXP

    [ModuleInitializer]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddAttachmentEvents() =>
        VerifierSettings.AddTestAttachment(AddFile);

    public static readonly AsyncLocal<TestExecutionContext?> CurrentTestContext = new();

    public static InnerVerifier BuildVerifier(VerifySettings settings, string sourceFile, bool useUniqueDirectory = false, int lineNumber = 0)
    {
        if (useUniqueDirectory)
        {
            settings.UseUniqueDirectory();
        }

        var context = GetTestContext();

        var assembly = context.Assembly;
        var type = context.TestClass;
        var method = context.Method;

        if (!settings.HasParameters)
        {
            var data = context.TestContext.TestData;
            // Only apply when the data length matches the method parameter count.
            // A params-array DataRow exposes raw pre-binding data whose length does
            // not match the parameter count, which would break parameterized
            // snapshot file naming. XunitV3 applies the same guard.
            if (data != null &&
                data.Length == method.ParameterNames()?.Count)
            {
                settings.SetParameters(data);
            }
        }

        VerifierSettings.AssignTargetAssembly(assembly);
        var pathInfo = GetPathInfo(sourceFile, type, method);
        return new(
            sourceFile,
            settings,
            type.NameWithParent(),
            method.Name,
            method.ParameterNames(),
            pathInfo,
            lineNumber);
    }

    internal static TestExecutionContext GetTestContext()
    {
        var context = CurrentTestContext.Value;
        if (context is not null)
        {
            return context;
        }

        // The ambient context is the normal path. CurrentTestContext is only set when something
        // outside this assembly assigns it, so it is checked first and otherwise unused.
        var ambient = AmbientContext;
        if (ambient is null)
        {
            throw new("TestContext is null. Ensure Verify is called from within a running MSTest test method.");
        }

        var className = ambient.FullyQualifiedTestClassName;
        if (className is null)
        {
            throw new("Expected TestContext.FullyQualifiedTestClassName to have a non null value.");
        }

        return new(ambient, TestClassResolver.Resolve(className, ambient.TestName));
    }

    [Pure]
    public static SettingsTask Verify(
        object? target,
        IEnumerable<Target> rawTargets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.Verify(target, rawTargets));

    [Pure]
    public static SettingsTask Verify(
        IEnumerable<Target> targets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.Verify(targets));

    [Pure]
    public static SettingsTask Verify(
        Target target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "",
        [CallerLineNumber] int lineNumber = 0) =>
        Verify(settings, sourceFile, lineNumber, _ => _.Verify(target));

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
            async settings =>
            {
                using var verifier = BuildVerifier(settings, sourceFile, useUniqueDirectory, lineNumber);
                return await verify(verifier);
            });
    }
}