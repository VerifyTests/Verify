[TestFixture]
public class Base
{
    [Test]
    public Task TestInBase() =>
        Verify("Foo");

    [Test]
    public virtual Task TestToOverride() =>
        Verify("Foo");
}