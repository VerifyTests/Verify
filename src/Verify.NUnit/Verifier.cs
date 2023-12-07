namespace VerifyNUnit;

public static partial class Verifier
{
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

        if (!settings.HasParameters &&
            adapter.Arguments.Length > 0)
        {
#pragma warning disable VerifySetParameters
            settings.SetParameters(adapter.Arguments);
#pragma warning restore
        }

        var customName = !adapter.FullName.StartsWith($"{testMethod.TypeInfo.FullName}.{testMethod.Name}");
        if (customName)
        {
            settings.typeName ??= adapter.GetTypeName();

            settings.methodName ??= adapter.GetMethodName();
        }

        var type = testMethod.TypeInfo.Type;
        VerifierSettings.AssignTargetAssembly(type.Assembly);

        var method = testMethod.MethodInfo;

        var pathInfo = GetPathInfo(sourceFile, type, method);
        return new(
            sourceFile,
            settings,
            type.NameWithParent(),
            method.Name,
            method.ParameterNames(),
            pathInfo);
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
        var fullName = adapter.FullName;
        var fullNameLength = fullName.Length - (adapter.Name.Length + 1);
        var typeName = fullName[..fullNameLength];
        var typeInfo = adapter.Method!.TypeInfo;
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (typeInfo.Namespace is not null)
        {
            typeName = typeName[(typeInfo.Namespace.Length + 1)..];
        }

        return typeName
            .Remove("\"")
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