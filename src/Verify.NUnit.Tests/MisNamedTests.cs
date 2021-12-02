[TestFixture]
public class DiffNamedTests
{
    [Test]
    public Task ShouldPass()
    {
        return Verifier.Verify("Foo");
    }
}