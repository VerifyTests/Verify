public class UserMachineScrubberChunkTests
{
    [Fact]
    public void CrossChunkMatchEndingExactlyAtChunkBoundary()
    {
        // "ABCD" fills the capacity-4 chunk; the next append lands in a fresh
        // 16-char chunk holding the remaining 16 chars of the 20-char match, so
        // the match ends exactly at that chunk's boundary with a chunk after it.
        // The trailing-char check must not read chunkSpan[chunkSpan.Length].
        var builder = new StringBuilder(capacity: 4);
        builder.Append("ABCD");
        builder.Append("EFGHIJKLMNOPQRST");
        builder.Append('.');

        UserMachineScrubber.PerformReplacements(builder, "ABCDEFGHIJKLMNOPQRST", "TheUserName");

        Assert.Equal("TheUserName.", builder.ToString());
    }

    [Fact]
    public void TokenSpanningThreeChunks()
    {
        // Capacity 4 forces small chunks [4][4][…]. The 10-char match spans all
        // three, and the 4-char middle chunk is shorter than the match. The
        // carryover must accumulate across chunks; otherwise the first chunk's
        // prefix is dropped when the short middle chunk overwrites it, and the
        // match is never found (silent leak).
        var find = "ABCDEFGHIJ"; // 10 chars
        var builder = new StringBuilder(capacity: 4);
        builder.Append("ABCD"); // chunk0 = find[0..4]
        builder.Append("EFGH"); // chunk1 = find[4..8] (short middle chunk)
        builder.Append("IJ.");  // chunk2 = find[8..10] + wrapper

        UserMachineScrubber.PerformReplacements(builder, find, "TheUserName");

        Assert.Equal("TheUserName.", builder.ToString());
    }
}
