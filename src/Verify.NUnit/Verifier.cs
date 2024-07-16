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
        var parameterNames = method.ParameterNames();
        var type = testMethod.TypeInfo.Type;

        if (!settings.HasParameters)
        {
            var test = GetTest(adapter);
            var parent = test.Parent;
            if (parent != null)
            {
                var argumentsLength = parent.Arguments.Length;
                if (argumentsLength > 0)
                {
                    var constructor = GetConstructorByParameterCount(type, argumentsLength);
                    var names = constructor
                        .GetParameters()
                        .Select(_ => _.Name!);
                    if (parameterNames == null)
                    {
                        parameterNames = names.ToList();
                    }
                    else
                    {
                        parameterNames.InsertRange(0, names);
                    }
                }
            }

            if (adapter.Arguments.Length > 0)
            {
                settings.SetParameters(adapter.Arguments);
            }
        }

        var customName = !adapter.FullName.StartsWith($"{testMethod.TypeInfo.FullName}.{testMethod.Name}");
        if (customName)
        {
            settings.typeName ??= adapter.GetTypeName();

            settings.methodName ??= adapter.GetMethodName();
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

    static ConstructorInfo GetConstructorByParameterCount(Type type, int argumentsLength)
    {
        var constructors = type
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
            .Where(_ => _.GetParameters()
                .Length == argumentsLength)
            .ToList();

        if (constructors.Count == 0)
        {
            throw new($"Could not find constructor with {argumentsLength} parameters.");
        }

        if (constructors.Count > 1)
        {
            throw new($"Found multiple constructor with {argumentsLength} parameters. Unable to derive names of parameters. Instead use UseParameters to pass in explicit parameter.");
        }

        return constructors[0];
    }

    static Test GetTest(TestContext.TestAdapter adapter)
    {
        var field = adapter
            .GetType()
            .GetField("_test", BindingFlags.Instance | BindingFlags.NonPublic)!;
        return (Test) field.GetValue(adapter)!;
    }

    static string GetMethodName(this TestContext.TestAdapter adapter)
    {
        var name = adapter.Name;
        var indexOf = name.IndexOf('(');

        if (indexOf != -1)
        {
            name = name[..indexOf];
        }

        return name.ReplaceInvalidFileNameChars();
    }

    static string GetTypeName(this TestContext.TestAdapter adapter)
    {
        var fullName = adapter.FullName.AsSpan();
        var fullNameLength = fullName.Length - (adapter.Name.Length + 1);
        var typeName = fullName[..fullNameLength];
        var typeInfo = adapter.Method!.TypeInfo;
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (typeInfo.Namespace is not null)
        {
            typeName = typeName[(typeInfo.Namespace.Length + 1)..];
        }

        return typeName
            .ToString()
            .Replace("\"", "")
            .ReplaceInvalidFileNameChars();
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