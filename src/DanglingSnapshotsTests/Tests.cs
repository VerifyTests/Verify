[TestFixture]
public class Tests
{
    [Test]
    public Task Simple() =>
        Verify("Foo");
}