using BenchmarkDotNet.Configs;

// StreamComparer is the default comparer for every binary snapshot, and on a passing run
// both files are read end to end, so the read path is the whole cost. The branch changed
// three things, and the ladder below isolates each one:
//  * Legacy_*        8K buffer, byte[] overload, sequential reads. The implementation as
//                    it stood on main.
//  * Legacy64K_*     only the buffer size changed, so a snapshot of any usual size is a
//                    couple of reads instead of dozens.
//  * LegacyMemory_*  64K plus the Memory<byte> ReadAsync overload, which drops the Task
//                    the byte[] overload allocates per call on an async FileStream.
//  * Current_*       the shipped implementation: the above plus the two reads overlapped.
//
// The verified side is always an async FileStream, because InnerCompare opens it with
// IoHelpers.OpenRead. The received side is a FileStream in the usual case, and a
// MemoryStream when FileComparer had to buffer a non seekable one, which is the Buffered
// category. Both sides are opened once and rewound per invocation, matching FileComparer,
// which reads both streams from position 0.
//
// The OS file cache is warm after the first iteration, so these measure the async read
// machinery rather than the disk. Overlapping is worth more than this shows when the
// reads reach storage.
//
// Grouped by category so each file size carries its own baseline. A ratio against one
// shared baseline would be comparing a 2K compare to a 1M one.
[MemoryDiagnoser]
[SimpleJob(iterationCount: 10, warmupCount: 3)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class StreamComparerBenchmarks
{
    const int oldBufferSize = 1024 * sizeof(long);
    const int newBufferSize = 64 * 1024;

    // Sized to the snapshots actually seen: a text snapshot is a couple of KB, a small
    // image or serialized document lands around the new buffer size, and a large binary
    // snapshot runs to megabytes.
    const int smallSize = 2 * 1024;
    const int mediumSize = 64 * 1024;
    const int largeSize = 1024 * 1024;

    string directory = null!;

    Pair small = null!;
    Pair medium = null!;
    Pair large = null!;
    Pair mismatch = null!;

    MemoryStream buffered = null!;

    [GlobalSetup]
    public void Setup()
    {
        directory = Path.Combine(Path.GetTempPath(), "VerifyStreamComparerBenchmarks");
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }

        Directory.CreateDirectory(directory);

        small = BuildPair("small", smallSize, differAtStart: false);
        medium = BuildPair("medium", mediumSize, differAtStart: false);
        large = BuildPair("large", largeSize, differAtStart: false);

        // Same length, first byte differs. FileComparer short circuits on a length
        // difference, so a mismatch that reaches here is usually an equal length one.
        mismatch = BuildPair("mismatch", largeSize, differAtStart: true);

        // FileComparer buffers a non seekable received stream into a MemoryStream, then
        // compares it against the verified file. So the received side reads synchronously
        // while the verified side is still async file IO. A MemoryStream on both sides
        // never reaches StreamComparer: InnerCompare always opens the verified side with
        // IoHelpers.OpenRead. Same content as the medium pair, so this compares equal
        // against that pair's verified file.
        buffered = new(BuildContent(mediumSize, seed: mediumSize));
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        small.Dispose();
        medium.Dispose();
        large.Dispose();
        mismatch.Dispose();
        buffered.Dispose();
        Directory.Delete(directory, true);
    }

    [BenchmarkCategory("Small")]
    [Benchmark(Baseline = true)]
    public async Task<bool> Legacy_Small() =>
        (await LegacyStreamComparer.AreEqual(small.Rewind(), small.Verified, oldBufferSize, useMemoryOverload: false)).IsEqual;

    [BenchmarkCategory("Small")]
    [Benchmark]
    public async Task<bool> Legacy64K_Small() =>
        (await LegacyStreamComparer.AreEqual(small.Rewind(), small.Verified, newBufferSize, useMemoryOverload: false)).IsEqual;

    [BenchmarkCategory("Small")]
    [Benchmark]
    public async Task<bool> LegacyMemory_Small() =>
        (await LegacyStreamComparer.AreEqual(small.Rewind(), small.Verified, newBufferSize, useMemoryOverload: true)).IsEqual;

    [BenchmarkCategory("Small")]
    [Benchmark]
    public async Task<bool> Current_Small() =>
        (await StreamComparer.AreEqual(small.Rewind(), small.Verified)).IsEqual;

    [BenchmarkCategory("Medium")]
    [Benchmark(Baseline = true)]
    public async Task<bool> Legacy_Medium() =>
        (await LegacyStreamComparer.AreEqual(medium.Rewind(), medium.Verified, oldBufferSize, useMemoryOverload: false)).IsEqual;

    [BenchmarkCategory("Medium")]
    [Benchmark]
    public async Task<bool> Legacy64K_Medium() =>
        (await LegacyStreamComparer.AreEqual(medium.Rewind(), medium.Verified, newBufferSize, useMemoryOverload: false)).IsEqual;

    [BenchmarkCategory("Medium")]
    [Benchmark]
    public async Task<bool> LegacyMemory_Medium() =>
        (await LegacyStreamComparer.AreEqual(medium.Rewind(), medium.Verified, newBufferSize, useMemoryOverload: true)).IsEqual;

    [BenchmarkCategory("Medium")]
    [Benchmark]
    public async Task<bool> Current_Medium() =>
        (await StreamComparer.AreEqual(medium.Rewind(), medium.Verified)).IsEqual;

    [BenchmarkCategory("Large")]
    [Benchmark(Baseline = true)]
    public async Task<bool> Legacy_Large() =>
        (await LegacyStreamComparer.AreEqual(large.Rewind(), large.Verified, oldBufferSize, useMemoryOverload: false)).IsEqual;

    [BenchmarkCategory("Large")]
    [Benchmark]
    public async Task<bool> Legacy64K_Large() =>
        (await LegacyStreamComparer.AreEqual(large.Rewind(), large.Verified, newBufferSize, useMemoryOverload: false)).IsEqual;

    [BenchmarkCategory("Large")]
    [Benchmark]
    public async Task<bool> LegacyMemory_Large() =>
        (await LegacyStreamComparer.AreEqual(large.Rewind(), large.Verified, newBufferSize, useMemoryOverload: true)).IsEqual;

    [BenchmarkCategory("Large")]
    [Benchmark]
    public async Task<bool> Current_Large() =>
        (await StreamComparer.AreEqual(large.Rewind(), large.Verified)).IsEqual;

    // A failing run: both sides are read once and the compare stops at the first chunk.
    [BenchmarkCategory("NotEqual")]
    [Benchmark(Baseline = true)]
    public async Task<bool> Legacy_Large_NotEqual() =>
        (await LegacyStreamComparer.AreEqual(mismatch.Rewind(), mismatch.Verified, oldBufferSize, useMemoryOverload: false)).IsEqual;

    [BenchmarkCategory("NotEqual")]
    [Benchmark]
    public async Task<bool> Current_Large_NotEqual() =>
        (await StreamComparer.AreEqual(mismatch.Rewind(), mismatch.Verified)).IsEqual;

    // The received side reads synchronously, so its read completes inline before the
    // verified read is even issued. The overlapping has nothing to hide behind here.
    [BenchmarkCategory("Buffered")]
    [Benchmark(Baseline = true)]
    public async Task<bool> Legacy_Medium_Buffered() =>
        (await LegacyStreamComparer.AreEqual(RewindBuffered(), medium.Verified, oldBufferSize, useMemoryOverload: false)).IsEqual;

    [BenchmarkCategory("Buffered")]
    [Benchmark]
    public async Task<bool> Legacy64K_Medium_Buffered() =>
        (await LegacyStreamComparer.AreEqual(RewindBuffered(), medium.Verified, newBufferSize, useMemoryOverload: false)).IsEqual;

    [BenchmarkCategory("Buffered")]
    [Benchmark]
    public async Task<bool> LegacyMemory_Medium_Buffered() =>
        (await LegacyStreamComparer.AreEqual(RewindBuffered(), medium.Verified, newBufferSize, useMemoryOverload: true)).IsEqual;

    [BenchmarkCategory("Buffered")]
    [Benchmark]
    public async Task<bool> Current_Medium_Buffered() =>
        (await StreamComparer.AreEqual(RewindBuffered(), medium.Verified)).IsEqual;

    MemoryStream RewindBuffered()
    {
        buffered.Position = 0;
        medium.Verified.Position = 0;
        return buffered;
    }

    Pair BuildPair(string name, int size, bool differAtStart)
    {
        var content = BuildContent(size, seed: size);
        var receivedPath = Path.Combine(directory, name + ".received.bin");
        var verifiedPath = Path.Combine(directory, name + ".verified.bin");
        File.WriteAllBytes(verifiedPath, content);

        if (differAtStart)
        {
            var copy = (byte[]) content.Clone();
            copy[0] ^= 0xFF;
            File.WriteAllBytes(receivedPath, copy);
        }
        else
        {
            File.WriteAllBytes(receivedPath, content);
        }

        return new(Open(receivedPath), Open(verifiedPath));
    }

    // Matches IoHelpers.OpenRead, which is how the verified side is opened in production.
    static FileStream Open(string path) =>
        new(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

    static byte[] BuildContent(int size, int seed)
    {
        var content = new byte[size];
        new Random(seed).NextBytes(content);
        return content;
    }

    sealed class Pair(FileStream received, FileStream verified) :
        IDisposable
    {
        public FileStream Verified { get; } = verified;

        // StreamComparer requires both streams at position 0. Rewinding costs the same
        // for every variant, so it does not skew the comparison.
        public FileStream Rewind()
        {
            received.Position = 0;
            Verified.Position = 0;
            return received;
        }

        public void Dispose()
        {
            received.Dispose();
            Verified.Dispose();
        }
    }
}
