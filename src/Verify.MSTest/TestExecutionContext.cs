namespace VerifyMSTest;

public record TestExecutionContext(TestContext TestContext, Type TestClass)
{
    public Assembly Assembly { get; } = TestClass.Assembly;
    public MethodInfo Method { get; } = FindMethod(TestClass, TestContext);

    static MethodInfo FindMethod(Type type, TestContext context)
    {
        var testName = context.TestName;

        if(testName is null)
        {
            throw new("Expected TestContext.TestName to have a non null value");
        }

        var span = testName.AsSpan();

        foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            if (span.SequenceEqual(method.Name))
            {
                return method;
            }
        }

        throw new($"Could not find method `{type.Name}.{testName}`.");
    }
}
