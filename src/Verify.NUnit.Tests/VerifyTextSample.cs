[TestFixture]
public class VerifyTextSample
{
    [Test]
    public Task Simple()
    {
        return Verify("Foo");
    }
}