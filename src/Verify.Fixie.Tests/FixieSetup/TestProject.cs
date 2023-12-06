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
                if (test.HasParameters)
                {
                    foreach (var parameters in test
                                 .GetAll<TestCase>()
                                 .Select(_ => _.Parameters))
                    {
                        using (ExecutionState.Set(testClass, test, parameters))
                        {
                            await test.Run(parameters);
                        }
                    }
                }
                else
                {
                    using (ExecutionState.Set(testClass, test, null))
                    {
                        await test.Run();
                    }
                }
            }
        }
    }
}