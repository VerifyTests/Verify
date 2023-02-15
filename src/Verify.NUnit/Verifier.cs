namespace VerifyNUnit;

public static partial class Verifier
{
    static FieldInfo field;

    static Verifier()
    {
        var temp = typeof(TestContext.TestAdapter)
            .GetField("_test", BindingFlags.Instance | BindingFlags.NonPublic);

        field = temp ?? throw new("Could not find field `_test` on TestContext.TestAdapter.");
    }

    static InnerVerifier BuildVerifier(string sourceFile, VerifySettings settings, bool useUniqueDirectory)
    {
        Guard.AgainstBadSourceFile(sourceFile);
        if (useUniqueDirectory)
        {
            settings.UseUniqueDirectory();
        }
        var context = TestContext.CurrentContext;
        var adapter = context.Test;
        var test = (Test) field.GetValue(adapter)!;
        var typeInfo = test.TypeInfo;
        if (typeInfo is null || test.Method is null)
        {
            throw new("Expected Test.TypeInfo and Test.Method to not be null. Raise a Pull Request with a test that replicates this problem.");
        }

        if (settings.parameters is null &&
            adapter.Arguments.Length > 0)
        {
            settings.parameters = adapter.Arguments;
        }

        if (test.IsCustomName())
        {
            settings.typeName ??= test.GetTypeName();

            settings.methodName ??= test.GetMethodName();
        }

        var type = typeInfo.Type;
        TargetAssembly.Assign(type.Assembly);

        var method = test.Method.MethodInfo;

        var pathInfo = GetPathInfo(sourceFile, type, method);
        return new(
            sourceFile,
            settings,
            type.NameWithParent(),
            method.Name,
            method.ParameterNames(),
            pathInfo);
    }

    static string GetMethodName(this ITest test)
    {
        var name = test.Name;
        var indexOf = name.IndexOf('(');

        if (indexOf != -1)
        {
            name = name[..indexOf];
        }

        return name.ReplaceInvalidFileNameChars();
    }

    static string GetTypeName(this Test test)
    {
        var fullName = test.FullName;
        var fullNameLength = fullName.Length - (test.Name.Length + 1);
        var typeName = fullName[..fullNameLength];
        var typeInfo = test.TypeInfo!;
        if (typeInfo.Namespace is not null)
        {
            typeName = typeName[(typeInfo.Namespace.Length + 1)..];
        }

        return typeName
            .Remove("\"")
            .ReplaceInvalidFileNameChars();
    }

    static bool IsCustomName(this ITest test) =>
        !test.FullName.StartsWith($"{test.TypeInfo!.FullName}.{test.Method!.Name}");

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

    public static SettingsTask Verify(
        object? target,
        IEnumerable<Target> rawTargets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            sourceFile,
            _ => _.Verify(target, rawTargets));

    public static SettingsTask Verify(
        IEnumerable<Target> targets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            sourceFile,
            _ => _.Verify(targets));

    public static SettingsTask Verify(
        Target target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(
            settings,
            sourceFile,
            _ => _.Verify(target));
}