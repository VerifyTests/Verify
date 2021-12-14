[UsesVerify]
public class Tests
{
    [Fact]
    public Task Simple()
    {
        return Verify("Foo");
    }
}