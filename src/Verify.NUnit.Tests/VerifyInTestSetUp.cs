[TestFixture]
public class VerifyInTestSetUp
{
    [SetUp]
    public Task SetUp() =>
        Verify("verify in a SetUp method, and the test fixture and test method is available for the verify file name.");

    [Test]
    public void IsVerified() =>
        Pass("Setup method is invoked before each test");
}