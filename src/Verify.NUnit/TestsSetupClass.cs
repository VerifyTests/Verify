#pragma warning disable InnerVerifyChecks
namespace VerifyNUnit;

[SetUpFixture]
public class TestsSetupClass
{
    [OneTimeSetUp]
    public void GlobalSetup()
    {
        // Do login here.
    }

    [OneTimeTearDown]
    public void GlobalTeardown() =>
        InnerVerifyChecks.Complete();

}