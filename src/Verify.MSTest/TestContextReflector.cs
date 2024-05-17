namespace VerifyMSTest;

static class TestContextReflector
{
    const BindingFlags ReflectionBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    static readonly ConcurrentDictionary<string, Type?> TypeCache = new();

    public static (Assembly Assembly, Type Type, MethodInfo Method) Get(TestContext context)
    {
        var testName = context.TestName;
        var typeName = context.FullyQualifiedTestClassName!; // Checked for null in the Verifier

        if (!TryGetTypeFromTestContext(typeName, context, out var type))
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

        var method = FindMethod(type, testNameSpan);

        return (type.Assembly, type, method);
    }

    /// <summary>
    /// As an optimization, try to retrieve the <see cref="ITestMethod"/> stored on the
    /// <see cref="TestExecutionContext"/>, and use that to retrieve the <see cref="Type"/>.
    ///
    /// If reflection fails, return <c>false</c>.
    /// </summary>
    /// <param name="typeName">The fully qualified name of the class of the currently running test.</param>
    /// <param name="testContext">The <see cref="TestExecutionContext"/> of the current test.</param>
    /// <param name="type">The <see cref="Type"/> of the currently running test.</param>
    /// <returns><c>true</c> if the reflection succeeded; <c>false</c> otherwise.</returns>
    static bool TryGetTypeFromTestContext(string typeName, TestContext testContext, [NotNullWhen(true)] out Type? type)
    {
        try
        {
            // We can't use UnsafeAccessor in this context because _testMethod is of type
            // Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter.ObjectModel.TestMethod, which isn't part of the MSTest.TestFramework
            // package. If / when an API like https://github.com/dotnet/runtime/issues/90081 lands this generic reflection can be replaced.
            var testMethod = testContext
                .GetType()
                .GetField("_testMethod", ReflectionBindingFlags)?.GetValue(testContext);
            var assemblyPath = testMethod
                ?.GetType()
                .GetProperty("AssemblyName", ReflectionBindingFlags)
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
        var result = TypeCache.GetOrAdd(typeName, name =>
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
}
