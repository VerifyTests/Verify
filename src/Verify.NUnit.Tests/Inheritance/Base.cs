[TestFixture]
public class Base
{
    [Test]
    public Task TestInBase()
    {
        return Verifier.Verify("Foo");
    }

    [Test]
    public virtual Task TestToOverride()
    {
        return Verifier.Verify("Foo");
    }
}