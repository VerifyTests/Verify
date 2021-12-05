[UsesVerify]
public class Tests
{
    [Fact]
    public Task Simple()
    {
        return Verifier.Verify("Foo");
    }
}