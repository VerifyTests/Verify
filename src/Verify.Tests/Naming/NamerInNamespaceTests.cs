namespace TheNamespace.Bar;

[UsesVerify]
public class NamerInNamespaceTests
{
    [Fact]
    public Task Run() =>
        Verify("value");
}