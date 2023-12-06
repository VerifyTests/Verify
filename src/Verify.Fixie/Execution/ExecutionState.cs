namespace VerifyFixie;

public class ExecutionState(TestClass testClass, Test test, object[]? parameters)
{
    public TestClass TestClass { get; } = testClass;
    public Test Test { get; } = test;
    public object[]? Parameters { get; } = parameters;
    static AsyncLocal<ExecutionState?> asyncLocal = new();

    public static IDisposable Set(TestClass testClass, Test test, object[]? parameters)
    {
        asyncLocal.Value = new(testClass, test, parameters);
        return new Cleanup(() => asyncLocal.Value = null);
    }

    public static ExecutionState Current
    {
        get
        {
            var state = asyncLocal.Value;
            if (state != null)
            {
                return state;
            }

            throw new(
                """
                No State found. Ensure a class inheriting from VerifyTestProject exists in the test project.
                public class TestProject :
                    VerifyTestProject;
                """);
        }
    }
}