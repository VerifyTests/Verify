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

        var indexOf = testName.IndexOf('(');
        if (indexOf > 0)
        {
            testName = testName[..indexOf];
        }

        indexOf = testName.IndexOf('.');
        if (indexOf > 0)
        {
            testName = testName[(indexOf + 1)..];
        }

        TargetAssembly.Assign(type.Assembly);
        var method = type
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(_ => _.Name == testName);

        if (method is null)
        {
            throw new($"Could not find method `{type.Name}.{testName}`.");
        }

        var pathInfo = GetPathInfo(sourceFile, type, method);
        return new(
            sourceFile,
            settings,
            type.NameWithParent(),
            method.Name,
            method.ParameterNames(),
            pathInfo);
    }

    public SettingsTask Verify(
        object? target,
        IEnumerable<Target> rawTargets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.Verify(target, rawTargets));

    public SettingsTask Verify(
        IEnumerable<Target> targets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.Verify(targets));

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