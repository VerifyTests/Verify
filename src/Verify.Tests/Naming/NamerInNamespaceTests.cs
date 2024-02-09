namespace TheNamespace.Bar;

public class NamerInNamespaceTests
{
    [Fact]
    public Task Run() =>
        Verify("value");
}