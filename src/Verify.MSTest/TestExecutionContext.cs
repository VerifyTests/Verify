namespace VerifyMSTest;

public record class TestExecutionContext
{
    public TestContext TestContext { get; }
    public Assembly TestAssembly { get; }
    public Type TestClass { get; }
    public MethodInfo TestMethod { get; }

    public TestExecutionContext(TestContext testContext, Type classType)
    {
        TestContext = testContext;
        TestAssembly = classType.Assembly;
        TestClass = classType;
        TestMethod = FindMethod(classType, testContext.TestName.AsSpan());
    }

    static MethodInfo FindMethod(Type type, ReadOnlySpan<char> testName)
    {
        foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            if (testName.SequenceEqual(method.Name))
            {
                return method;
            }
        }

        throw new($"Could not find method `{type.Name}.{testName.ToString()}`.");
    }
}
