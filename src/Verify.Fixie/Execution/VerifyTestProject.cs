namespace VerifyFixie;

public abstract class VerifyTestProject :
    ITestProject
{
    public void Configure(TestConfiguration configuration, TestEnvironment environment)
    {
        VerifierSettings.AssignTargetAssembly(environment.Assembly);
        configuration.Conventions.Add<DefaultDiscovery, VerifyExecution>();
    }
}