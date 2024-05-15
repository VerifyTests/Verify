namespace VerifyMSTest;

public static partial class Verifier
{
    static ConcurrentDictionary<string, Type?> typeCache = new();
    const string AttributeUsageHelp = "Ensure test class has a `[UsesVerify]` attribute.";

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
            throw new($"TestContext is null. {AttributeUsageHelp}");
        }

        var testName = CurrentTestContext.Value.TestName;
        if (testName is null)
        {
            throw new($"TestContext.TestName is null. {AttributeUsageHelp}");
        }

        var typeName = CurrentTestContext.Value.FullyQualifiedTestClassName;
        if (typeName is null)
        {
            throw new($"TestContext.FullyQualifiedTestClassName is null. {AttributeUsageHelp}");
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

    /// <summary>
    /// As an optimization, try to retrieve the <see cref="ITestMethod"/> stored on the
    /// <see cref="TestContext"/>, and use that to retrieve the <see cref="Type"/>.
    ///
    /// If reflection fails, return <c>false</c>.
    /// </summary>
    /// <param name="typeName">The fully qualified name of the class of the currently running test.</param>
    /// <param name="testContext">The <see cref="TestContext"/> of the current test.</param>
    /// <param name="type">The <see cref="Type"/> of the currently running test.</param>
    /// <returns><c>true</c> if the reflection succeeded; <c>false</c> otherwise.</returns>
    static bool TryGetTypeFromTestContext(string typeName, TestContext testContext, [NotNullWhen(true)] out Type? type)
    {
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

            type = Type.GetType($"{typeName}, {assemblyName}", throwOnError: false);

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

    /// <summary>
    /// Get the <see cref="Type"/> of the test class from the fully qualified name.
    /// </summary>
    /// <param name="typeName">The fully qualified class name of the currently running test.</param>
    /// <returns>The <see cref="Type"/> for the currently running test class.</returns>
    /// <remarks>
    /// Uses a <see cref="ConcurrentDictionary{TKey, TValue}"/> to avoid repeated lookups.
    /// This method should only be used as a fallback if reflection fails because:
    ///   1. It's slower
    ///   2. The type cache can grow large for large test suites
    /// </remarks>
    static Type FindType(string typeName)
    {
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