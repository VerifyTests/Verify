[TestFixture]
public class Base
{
    [Test]
    public Task TestInBase()
    {
        return Verify("Foo");
    }

    [Test]
    public virtual Task TestToOverride()
    {
        return Verify("Foo");
    }
}