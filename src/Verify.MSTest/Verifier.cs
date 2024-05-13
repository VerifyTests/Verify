using System.Reflection;

namespace VerifyMSTest;

public static partial class Verifier
{
    static Task AddFile(FilePair path, bool autoVerify)
    {
        var context = CurrentTestContext.Value;
        if (context != null)
        {
            var fileName = autoVerify ? path.VerifiedPath : path.ReceivedPath;
            context.AddResultFile(fileName);
        }

        return Task.CompletedTask;
    }

    static Verifier()
    {
        VerifierSettings.OnFirstVerify((pair, _, autoVerify) => AddFile(pair, autoVerify));
        VerifierSettings.OnVerifyMismatch((pair, _, autoVerify) => AddFile(pair, autoVerify));
    }

    public static readonly AsyncLocal<TestContext?> CurrentTestContext = new();

    static InnerVerifier BuildVerifier(VerifySettings settings, string sourceFile, bool useUniqueDirectory)
    {
        if (useUniqueDirectory)
        {
            settings.UseUniqueDirectory();
        }

        var testName = CurrentTestContext.Value?.TestName;
        if (testName == null)
        {
            throw new("TestContext.TestName is null. Ensure test class has a `[UsesVerify]` attribute.");
        }

        var typeName = CurrentTestContext.Value?.FullyQualifiedTestClassName;
        var type = FindType(typeName.AsSpan());

        VerifierSettings.AssignTargetAssembly(type.Assembly);
        var method = FindMethod(type, testName.AsSpan());

        sourceFile = IoHelpers.GetMappedBuildPath(sourceFile);
        var fileName = Path.GetFileNameWithoutExtension(sourceFile);

        var pathInfo = GetPathInfo(sourceFile, type, method);
        return new(sourceFile, settings, fileName, testName, method.ParameterNames(), pathInfo);
    }

    static Type FindType(ReadOnlySpan<char> typeName)
    {
        // TODO: Either
        //    1. Switch to the other DerivePathInfo that uses names (e.g. from Expecto) and avoid type lookups
        //    2. Add a cache here to speed up the lookup
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .Reverse()
            .SelectMany(assembly => assembly.GetTypes());

        foreach (var type in types)
        {
            if (typeName.SequenceEqual(type.FullName))
            {
                return type;
            }
        }

        throw new($"Type '{typeName}' from TestContext not found.");
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
    public static SettingsTask Verify(
        object? target,
        IEnumerable<Target> rawTargets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.Verify(target, rawTargets));

    [Pure]
    public static SettingsTask Verify(
        IEnumerable<Target> targets,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.Verify(targets));

    [Pure]
    public static SettingsTask Verify(
        Target target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.Verify(target));

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
                using var verifier = BuildVerifier(verifySettings, sourceFile, useUniqueDirectory);
                return await verify(verifier);
            });
    }
}