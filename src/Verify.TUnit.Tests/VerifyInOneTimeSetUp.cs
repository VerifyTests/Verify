[TestFixture]
public class VerifyInOneTimeSetUp
{
    [OneTimeSetUp]
    public void SetUp()
    {
        var exception = ThrowsAsync<Exception>(
            () => Verify("If I call verify in a fixtures One Time SetUp method, then test method is not available for the verify file name."))!;
        That(exception.Message, Is.EqualTo("TestContext.CurrentContext.Test.Method is null. Verify can only be used from within a test method."));
    }

    [Test]
    public void ThrowsInvalidOperationException() =>
        Pass("Verify is not available");
}