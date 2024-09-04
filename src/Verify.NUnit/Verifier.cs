#pragma warning disable VerifySetParameters
namespace VerifyNUnit;

public static partial class Verifier
{
    static Task AddFile(FilePair path)
    {
        TestContext.AddTestAttachment(path.ReceivedPath);
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

        var adapter = TestContext.CurrentContext.Test;

        var testMethod = adapter.Method;
        if (testMethod is null)
        {
            throw new("TestContext.CurrentContext.Test.Method is null. Verify can only be used from within a test method.");
        }

        var method = testMethod.MethodInfo;
        var type = testMethod.TypeInfo.Type;

        IReadOnlyList<string>? parameterNames;
        if (settings.HasParameters)
        {
            parameterNames = adapter.GetParameterNames();
        }
        else
        {
            var (names, values) = GetParameterInfo(adapter);
            settings.SetParameters(values);
            parameterNames = names;
        }

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

    static (IReadOnlyList<string>? names, object?[] values) GetParameterInfo(TestAdapter adapter)
    {
        var method = adapter.Method!;

        var methodParameterNames = method.MethodInfo.ParameterNames();

        if (!adapter.TryGetParent(out var parent))
        {
            return (methodParameterNames, adapter.Arguments);
        }

        var argumentsLength = parent.Arguments.Length;
        if (argumentsLength == 0)
        {
            return (methodParameterNames, adapter.Arguments);
        }

        var names = method.TypeInfo.Type.GetConstructorParameterNames(argumentsLength);
        if (methodParameterNames == null)
        {
            return (names.ToList(), parent.Arguments);
        }

        return (
            [.. names, .. methodParameterNames],
            [.. parent.Arguments, .. adapter.Arguments]);
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