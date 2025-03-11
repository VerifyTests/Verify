#region XunitDanglingCollection
[Collection(nameof(SharedFixtureCollection))]
public class Tests
{
    [Fact]
    public Task Simple() =>
        Verify("Foo");
#endregion

    [Fact]
    public Task IncorrectCase() =>
        Verify("Foo");
}

