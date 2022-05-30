[TestFixture]
public class DiffNamedTests
{
    [Test]
    public Task ShouldPass() =>
        Verify("Foo");
}