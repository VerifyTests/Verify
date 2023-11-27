[TestFixture]
public class VerifyInTestSetUp
{
    [SetUp]
    public Task SetUp() =>
        Verify("verify in a SetUp method, and the test fixture and test method is available for the verify file name.");

    [Test]
    public void IsVerified() =>
        Assert.Pass("Setup method is invoked before each test");
}

public class VerifyInOneTimeSetUp
{
    [OneTimeSetUp]
    public void SetUp()
    {
        var exception = Assert.ThrowsAsync<Exception>(
            () => Verify("If I call verify in a fixtures One Time SetUp method, then test method is not available for the verify file name."))!;
        Assert.That(exception.Message, Is.EqualTo("TestContext.CurrentContext.Test.Method is null. Verify can only be used from within a test method."));
    }

    [Test]
    public void ThrowsInvalidOperationException() =>
        Assert.Pass("Verify is not available");
}