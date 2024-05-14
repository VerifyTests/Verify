namespace VerifyMSTest;

public static partial class Verifier
{
    private static ConcurrentDictionary<string, Type?> typeCache = new();

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

        if (CurrentTestContext.Value is null)
        {
            throw new("TestContext is null. Ensure test class has a `[UsesVerify]` attribute.");
        }

        var testName = CurrentTestContext.Value.TestName;
        if (testName is null)
        {
            throw new("TestContext.TestName is null. Ensure test class has a `[UsesVerify]` attribute.");
        }

        var typeName = CurrentTestContext.Value.FullyQualifiedTestClassName;
        if (typeName is null)
        {
            throw new("TestContext.FullyQualifiedTestClassName is null. Ensure test class has a `[UsesVerify]` attribute.");
        }

        if(!TryGetTypeFromTestContext(typeName, CurrentTestContext.Value, out var type))
        {
            type = FindType(typeName);
        }

        var testNameSpan = testName.AsSpan();
        var indexOf = testNameSpan.IndexOf('(');
        if (indexOf > 0)
        {
            testNameSpan = testNameSpan[..indexOf];
        }

        indexOf = testNameSpan.IndexOf('.');
        if (indexOf > 0)
        {
            testNameSpan = testNameSpan[(indexOf + 1)..];
        }

        VerifierSettings.AssignTargetAssembly(type.Assembly);
        var method = FindMethod(type, testNameSpan);

        var pathInfo = GetPathInfo(sourceFile, type, method);
        return new(
            sourceFile,
            settings,
            type.NameWithParent(),
            method.Name,
            method.ParameterNames(),
            pathInfo);
    }

    private static bool TryGetTypeFromTestContext(string typeName, TestContext testContext, [NotNullWhen(true)] out Type? type)
    {
        // TODO: Should we file a bug here on testfx?

        try
        {
            var testMethod = testContext
                .GetType()
                .GetField("_testMethod", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.GetValue(testContext);
            var assemblyPath = testMethod
                ?.GetType()
                ?.GetProperty("AssemblyName", BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(testMethod);
            var assemblyName = Path.GetFileNameWithoutExtension(assemblyPath as string ?? string.Empty);

            type = Type.GetType($"{typeName}, {assemblyName}");

            if (type is not null)
            {
                return true;
            }
        }
        catch
        {
        }

        type = null;
        return false;
    }

    private static Type FindType(string typeName)
    {
        // TODO: Do we need the cache here?
        var result = typeCache.GetOrAdd(typeName, name =>
        {
            var nameSpan = name.AsSpan();
            var types = AppDomain.CurrentDomain.GetAssemblies()
            .Reverse()
            .SelectMany(assembly => assembly.GetTypes());

            foreach (var type in types)
            {
                if (nameSpan.SequenceEqual(type.FullName))
                {
                    return type;
                }
            }

            return null;
        });

        if (result is null)
        {
            throw new($"Type '{typeName}' from TestContext not found.");
        }

        return result;
    }

    private static MethodInfo FindMethod(Type type, ReadOnlySpan<char> testName)
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