public class UseComparerDifferentTargets
{
    [Fact]
    public async Task Test()
    {
        Target[] targets =
        [
            new("ComparerDifferentTargets1", new MemoryStream("1"u8.ToArray())),
            new("ComparerDifferentTargets2", new MemoryStream("2"u8.ToArray()))
        ];
        await Verify(null, targets)
            .UseStreamComparer(Compare1, "ComparerDifferentTargets1")
            .UseStreamComparer(Compare2, "ComparerDifferentTargets2");
        Assert.Equal(1, compare1CallCount);
        Assert.Equal(1, compare2CallCount);
    }

    static int compare1CallCount;

    static Task<CompareResult> Compare1(Stream received, Stream verified, IReadOnlyDictionary<string, object> context)
    {
        compare1CallCount++;
        var receivedByte = received.ReadByte();
        var verifiedByte = verified.ReadByte();
        return Task.FromResult(new CompareResult(receivedByte == verifiedByte));
    }

    static int compare2CallCount;

    static Task<CompareResult> Compare2(Stream received, Stream verified, IReadOnlyDictionary<string, object> context)
    {
        compare2CallCount++;
        var receivedByte = received.ReadByte();
        var verifiedByte = verified.ReadByte();
        return Task.FromResult(new CompareResult(receivedByte == verifiedByte));
    }
}