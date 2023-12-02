namespace VerifyFixie;

public class VerifyExecution : IExecution
{
    static AsyncLocal<State?> asyncLocal = new();

    public virtual async Task Run(TestSuite testSuite)
    {

        foreach (var testClass in testSuite.TestClasses)
        {
            foreach (var test in testClass.Tests)
            {
                using (SetState(testClass, test, null))
                {
                    await test.Run();
                }
            }
        }
    }

    public static IDisposable SetState(TestClass testClass, Test test, object[]? parameters)
    {
        asyncLocal.Value = new(testClass, test, parameters);
        return new Cleanup(() => asyncLocal.Value = null);
    }

    public static State State
    {
        get
        {
            var state = asyncLocal.Value;
            if (state == null)
            {
                throw new();
            }

            return state;
        }
    }
}