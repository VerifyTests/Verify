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
        Guard.AgainstBadSourceFile(sourceFile);
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
            parameterNames = GetParameterNames(adapter);
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

        var parent = GetParent(adapter);

        if (parent == null)
        {
            return (methodParameterNames, adapter.Arguments);
        }

        var argumentsLength = parent.Arguments.Length;
        if (argumentsLength == 0)
        {
            return (methodParameterNames, adapter.Arguments);
        }

        var names = GetConstructorParameterNames(method.TypeInfo.Type, argumentsLength);
        if (methodParameterNames == null)
        {
            return (names.ToList(), parent.Arguments);
        }

        return (
            [.. names, .. methodParameterNames],
            [.. parent.Arguments, .. adapter.Arguments]);
    }

    static IReadOnlyList<string>? GetParameterNames(TestAdapter adapter)
    {
        var method = adapter.Method!;

        var methodParameterNames = method.MethodInfo.ParameterNames();

        var parent = GetParent(adapter);

        if (parent == null)
        {
            return methodParameterNames;
        }

        var names = GetConstructorParameterNames(method.TypeInfo.Type, parent.Arguments.Length);
        if (methodParameterNames == null)
        {
            return names.ToList();
        }

        return [.. names, .. methodParameterNames];
    }

    static ITest? GetParent(TestAdapter adapter)
    {
        var test = GetTest(adapter);
        var parent = test.Parent;
        if (parent is ParameterizedMethodSuite methodSuite)
        {
            return methodSuite.Parent;
        }

        return parent;
    }

    static Test GetTest(TestAdapter adapter)
    {
        var field = adapter
            .GetType()
            .GetField("_test", BindingFlags.Instance | BindingFlags.NonPublic)!;
        return (Test) field.GetValue(adapter)!;
    }

    static IEnumerable<string> GetConstructorParameterNames(Type type, int argumentsLength)
    {
        IEnumerable<string>? names = null;
        foreach (var constructor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public))
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length != argumentsLength)
            {
                continue;
            }

            if (names != null)
            {
                throw new($"Found multiple constructors with {argumentsLength} parameters. Unable to derive names of parameters. Instead use UseParameters to pass in explicit parameter.");
            }

            names = parameters.Select(_ => _.Name!);
        }

        if (names == null)
        {
            throw new($"Could not find constructor with {argumentsLength} parameters.");
        }

        return names;
    }

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