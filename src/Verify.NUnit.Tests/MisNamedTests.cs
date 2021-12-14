[TestFixture]
public class DiffNamedTests
{
    [Test]
    public Task ShouldPass()
    {
        return Verify("Foo");
    }
}