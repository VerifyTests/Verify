using Polyfills;

namespace VerifyXunit;

public static partial class Verifier
{
    static async Task AddFile(FilePair pair)
    {
        var context = TestContext.Current;
        var path = pair.ReceivedPath;
        context.AddAttachment(
            $"Verify snapshot mismatch: {Path.GetFileName(path)}",
            await FilePolyfill.ReadAllBytesAsync(path));
    }

    static Verifier()
    {
        VerifierSettings.OnFirstVerify((pair, _, _) => AddFile(pair));
        VerifierSettings.OnVerifyMismatch((pair, _, _) => AddFile(pair));
    }

    static InnerVerifier BuildVerifier(VerifySettings settings, string sourceFile, bool useUniqueDirectory)
    {
        var context = TestContext.Current;
        var testMethod = context.GetTestMethod();

        var method = testMethod.Method;

        if (useUniqueDirectory)
        {
            settings.UseUniqueDirectory();
        }

        var parameterNames = method.ParameterNames();

        SetParametersFromContext(settings, context, parameterNames);

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

    static void SetParametersFromContext(VerifySettings settings, ITestContext context, IReadOnlyList<string>? parameterNames)
    {
        if (settings.HasParameters)
        {
            return;
        }

        if (context.TestCase is XunitTestCase {TestMethodArguments.Length: > 0} testCase &&
            testCase.TestMethodArguments.Length == parameterNames?.Count)
        {
            settings.SetParameters(testCase.TestMethodArguments);
            return;
        }

        //For some reason when using `dotnet run` then TestCase is not a XunitTestCase
        if (context.Test is XunitTest {TestMethodArguments.Length: > 0} test &&
            test.TestMethodArguments.Length == parameterNames?.Count)
        {
            settings.SetParameters(test.TestMethodArguments);
            return;
        }
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