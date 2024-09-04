using Xunit.v3;

namespace VerifyXunit;

public static partial class Verifier
{
    static InnerVerifier BuildVerifier(VerifySettings settings, string sourceFile, bool useUniqueDirectory)
    {
        var testContext = TestContext.Current;
        var testContextTestMethod = testContext.TestMethod;
        if (testContextTestMethod == null)
        {
            throw new("TestContext.TestMethod is null");
        }

        var testMethod = (IXunitTestMethod) testContextTestMethod;

        var method = testMethod.Method;

        if (useUniqueDirectory)
        {
            settings.UseUniqueDirectory();
        }

        var parameterNames = method.ParameterNames();

        if (!settings.HasParameters &&
            testContext.TestCase is XunitTestCase { TestMethodArguments.Length: > 0 } testCase &&
            testCase.TestMethodArguments.Length == parameterNames?.Count)
        {
            settings.SetParameters(testCase.TestMethodArguments);
        }

        var type = method.ReflectedType!;
        VerifierSettings.AssignTargetAssembly(type.Assembly);

        var pathInfo = GetPathInfo(sourceFile, type, method);
        return new(
            sourceFile,
            settings,
            type.NameWithParent(),
            method.Name,
            parameterNames,
            pathInfo);
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