public class VerifyInSetUp
{
    [SetUp]
    public async Task SetUp()
    {
        await Verify("I can call verify in SetUp, and the test method is known.");
    }

    [Test]
    public void Should_test()
    {
        Assert.Pass();
    }
}

public class VerifyInOneTimeSetUp
{
    [OneTimeSetUp]
    public async Task SetUp()
    {
        // var ex = Assert.ThrowsAsync<InvalidOperationException>(() => Verify("If I call verify in One Time SetUp the test method is not known. This should probably be an IllegalStateException"))!;
        // Assert.That(ex.Message, Is.EqualTo("Executing Verify in a One Time Setup method is not supported. Please run Verify in a test method."));
        await Verify(Verify("If I call verify in One Time SetUp the test method is not known. This should probably be an IllegalStateException"));
    }

    [Test]
    public void Should_test()
    {
        Assert.Pass();
    }
}