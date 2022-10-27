[UsesVerify]
public class VerifyTextSample
{
    [Fact]
    public Task Simple() =>
        Verify("Value");
}