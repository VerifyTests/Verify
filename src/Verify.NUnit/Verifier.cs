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

        var testMethod = adapter.GetTestMethod();

        var method = testMethod.MethodInfo;
        var type = testMethod.TypeInfo.Type;

        IReadOnlyList<string>? parameterNames;
        if (settings.HasParameters)
        {
            parameterNames = GetParameterNames(method, adapter);
        }
        else
        {
            (parameterNames, var parameters) = GetParametersAndNames(method, adapter);
            settings.SetParameters(parameters);
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

    static IReadOnlyList<string>? GetParameterNames(MethodInfo method, TestAdapter adapter)
    {
        var methodParameterNames = method.ParameterNames();
        return adapter.GetParameterNames(methodParameterNames);
    }

    static (IReadOnlyList<string>? names, object?[] parameters) GetParametersAndNames(MethodInfo method, TestAdapter adapter)
    {
        var methodParameterNames = method.ParameterNames();
        var parameterNames = adapter.GetParameterNames(methodParameterNames);
        if (!adapter.TryGetParent(out var parent))
        {
            return (parameterNames, adapter.Arguments);
        }

        var argumentsLength = parent.Arguments.Length;
        if (argumentsLength == 0)
        {
            return (parameterNames, adapter.Arguments);
        }

        if (methodParameterNames == null)
        {
            return (parameterNames, parent.Arguments);
        }

        return (
            parameterNames,
            [.. parent.Arguments, .. adapter.Arguments]);
    }

    static object?[] GetParameters(TestAdapter adapter, IReadOnlyList<string>? methodParameterNames)
    {
        if (!adapter.TryGetParent(out var parent))
        {
            return adapter.Arguments;
        }

        var argumentsLength = parent.Arguments.Length;
        if (argumentsLength == 0)
        {
            return adapter.Arguments;
        }

        if (methodParameterNames == null)
        {
            return parent.Arguments;
        }

        return [.. parent.Arguments, .. adapter.Arguments];
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