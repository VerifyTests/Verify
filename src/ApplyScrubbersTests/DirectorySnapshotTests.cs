// The anchors and the shortest find are derived from the pairs, so UseAssembly has
// to keep them in step. A scan that used anchors not covering a pair's first char
// would never land on that path.
public class DirectorySnapshotTests
{
    static DirectorySnapshotTests() =>
        EngineRunner.UseFakeDirectories();

    [Fact]
    public void AnchorsCoverEveryPair()
    {
        var snapshot = DirectoryReplacements.Current;
        Assert.NotEmpty(snapshot.Pairs);

        foreach (var pair in snapshot.Pairs)
        {
            Assert.Contains(pair.Find[0], snapshot.Anchors);
        }
    }

    [Fact]
    public void ShortestFindLengthMatchesThePairs()
    {
        var snapshot = DirectoryReplacements.Current;
        var shortest = int.MaxValue;
        foreach (var pair in snapshot.Pairs)
        {
            shortest = Math.Min(shortest, pair.Find.Length);
        }

        Assert.Equal(shortest, snapshot.ShortestFindLength);
    }

    [Fact]
    public void PairsAreOrderedLongestFirst()
    {
        // The most specific path has to win at any given position
        var snapshot = DirectoryReplacements.Current;
        for (var index = 1; index < snapshot.Pairs.Count; index++)
        {
            Assert.True(snapshot.Pairs[index - 1].Find.Length >= snapshot.Pairs[index].Find.Length);
        }
    }
}
