[CollectionDefinition(nameof(SharedFixtureCollection))]
public class SharedFixtureCollection :
    ICollectionFixture<SharedFixture>;

public sealed class SharedFixture :
    IDisposable
{
#pragma warning disable VerifyDanglingSnapshots
    public void Dispose() => DanglingSnapshots.Run();
#pragma warning restore VerifyDanglingSnapshots
}