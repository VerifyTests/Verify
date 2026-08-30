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
                No State found. Fixie leaves test execution up to the consumer, so Verify needs a class in the test project implementing Fixie's ITestProject and IExecution:
                  * ITestProject.Configure must call VerifierSettings.AssignTargetAssembly(environment.Assembly)
                  * IExecution.Run must wrap each test.Run in `using (ExecutionState.Set(testClass, test, parameters))`
                See https://github.com/VerifyTests/Verify/blob/main/docs/mdsource/fixie-convention.include.md for a full example.
                """);
        }
    }
}