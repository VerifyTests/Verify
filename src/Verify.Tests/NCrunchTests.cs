[UsesVerify]
public class NCrunchTests
{
    [Fact]
    public Task Simple() =>
        Verify("Value2s");
}