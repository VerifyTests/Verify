public class Base
{
    public Task TestInBase() =>
        Verify("Foo");

    public virtual Task TestToOverride() =>
        Verify("Foo");
}