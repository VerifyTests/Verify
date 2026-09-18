#pragma warning disable VerifyDanglingSnapshots

public class TestProject :
    ITestProject,
    IExecution
{
    public void Configure(TestConfiguration configuration, TestEnvironment environment)
    {
        VerifierSettings.AssignTargetAssembly(environment.Assembly);
        configuration.Conventions.Add<DefaultDiscovery, TestProject>();
    }

    public async Task Run(TestSuite testSuite)
    {
        foreach (var testClass in testSuite.TestClasses)
        {
            foreach (var test in testClass.Tests)
            {
                using (ExecutionState.Set(testClass, test, null))
                {
                    await test.Run();
                }
            }
        }

        DanglingSnapshots.Run();
    }
}
