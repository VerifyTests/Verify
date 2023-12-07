namespace VerifyMSTest;

[TestClass]
public abstract partial class VerifyBase
{
    public TestContext TestContext { get; set; } = null!;

    InnerVerifier BuildVerifier(VerifySettings settings, string sourceFile, bool useUniqueDirectory)
    {
        var type = GetType();

        if (useUniqueDirectory)
        {
            settings.UseUniqueDirectory();
        }

        var testName = TestContext.TestName;
        if (testName == null)
        {
            throw new("TestContext.TestName is null. Ensure being used inside a test");
        }

        var testNameSpan = testName.AsSpan();
        var indexOf = testNameSpan.IndexOf('(');
        if (indexOf > 0)
        {
            testNameSpan = testNameSpan[..indexOf];
        }

        indexOf = testNameSpan.IndexOf('.');
        if (indexOf > 0)
        {
            testNameSpan = testNameSpan[(indexOf + 1)..];
        }

        VerifierSettings.AssignTargetAssembly(type.Assembly);
        var method = FindMethod(type, testNameSpan);

        var pathInfo = GetPathInfo(sourceFile, type, method);
        return new(
            sourceFile,
            settings,
            type.NameWithParent(),
            method.Name,
            method.ParameterNames(),
            pathInfo);
    }

    static MethodInfo FindMethod(Type type, ReadOnlySpan<char> testName)
    {
        foreach (var method in type
                     .GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            if (testName.SequenceEqual(method.Name))
            {
                return method;
            }
        }

        throw new($"Could not find method `{type.Name}.{testName.ToString()}`.");
    }

    [Pure]
    public SettingsTask Verify(
        object? target,
        IEnumerable<Target> rawTargets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.Verify(target, rawTargets));

    [Pure]
    public SettingsTask Verify(
        IEnumerable<Target> targets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.Verify(targets));

    [Pure]
    public SettingsTask Verify(
        Target target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.Verify(target));

    SettingsTask Verify(
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