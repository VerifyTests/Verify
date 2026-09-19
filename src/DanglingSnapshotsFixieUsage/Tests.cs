public class Tests
{
    public Task Simple() =>
        Verify("Foo");

    public Task Second() =>
        Verify("Foo");
}
