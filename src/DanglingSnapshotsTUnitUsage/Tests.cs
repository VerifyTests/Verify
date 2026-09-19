public class Tests
{
    [Test]
    public Task Simple() =>
        Verify("Foo");

    [Test]
    public Task Second() =>
        Verify("Foo");
}
