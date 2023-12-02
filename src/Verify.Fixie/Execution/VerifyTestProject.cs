namespace VerifyFixie;

public abstract class VerifyTestProject : ITestProject
{
    public void Configure(TestConfiguration configuration, TestEnvironment environment)
    {
        TargetAssembly.Assign(environment.Assembly);
        configuration.Conventions.Add<DefaultDiscovery, VerifyExecution>();
    }
}