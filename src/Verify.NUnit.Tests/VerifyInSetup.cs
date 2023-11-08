public class VerifyInTestSetUp
{
    [SetUp]
    public async Task SetUp()
    {
        await Verify("I can call verify in a SetUp method, and the test fixture and test method is available for the verify file name.");
    }

    [Test]
    public void IsVerified()
    {
        Assert.Pass("Setup method is invoked before each test");
    }
}

public class VerifyInOneTimeSetUp
{
    [OneTimeSetUp]
    public void SetUp()
    {
        var ex = Assert.ThrowsAsync<InvalidOperationException>(() => Verify("If I call verify in a fixtures One Time SetUp method, then test method is not available for the verify file name."))!;
        Assert.That(ex.Message, Is.EqualTo("Executing Verify in a One Time Setup method is not supported. Please run Verify in a test method."));
    }

    [Test]
    public void ThrowsInvalidOperationException()
    {
        Assert.Pass("Verify is not available");
    }
}