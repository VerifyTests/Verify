namespace VerifyFixie;

public abstract class VerifyTestProject : ITestProject
{
    public void Configure(TestConfiguration configuration, TestEnvironment environment) =>
        configuration.Conventions.Add<DefaultDiscovery, VerifyExecution>();
}