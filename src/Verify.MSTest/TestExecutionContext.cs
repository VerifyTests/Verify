namespace VerifyMSTest;

public record TestExecutionContext(TestContext TestContext, Type TestClass)
{
    public Assembly TestAssembly { get; } = TestClass.Assembly;
    public MethodInfo TestMethod { get; } = FindMethod(TestClass, TestContext.TestName.AsSpan());

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
