[TestFixture]
public class VerifyTextSample
{
    [Test]
    public Task Simple() =>
        Verify("Foo");
}