namespace VerifyMSTest;

static class TestContextReflector
{
    public static (Assembly Assembly, Type Type, MethodInfo Method) Get(TestContext context, Type type)
    {
        var method = FindMethod(type, context.TestName.AsSpan());

        return (type.Assembly, type, method);
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
