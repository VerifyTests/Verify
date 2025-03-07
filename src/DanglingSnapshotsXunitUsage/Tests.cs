[Collection(nameof(SharedFixtureCollection))]
public class Tests
{
    [Fact]
    public Task Simple() =>
        Verify("Foo");

    [Fact]
    public Task IncorrectCase() =>
        Verify("Foo");
}

