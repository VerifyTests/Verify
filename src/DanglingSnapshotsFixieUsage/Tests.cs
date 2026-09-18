public class Tests
{
    public Task Simple() =>
        Verify("Foo");

    public Task IncorrectCase() =>
        Verify("Foo");
}
