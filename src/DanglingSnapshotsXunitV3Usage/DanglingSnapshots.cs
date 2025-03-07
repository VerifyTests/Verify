[CollectionDefinition(nameof(SharedFixtureCollection))]
public class SharedFixtureCollection :
    ICollectionFixture<SharedFixture>;

public sealed class SharedFixture :
    IDisposable
{
    public void Dispose() => DanglingSnapshots.Run();
}