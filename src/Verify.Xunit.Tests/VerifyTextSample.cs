[UsesVerify]
public class VerifyTextSample
{
    [Fact]
    public Task Simple()
    {
        return Verify("Foo");
    }
}