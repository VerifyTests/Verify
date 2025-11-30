[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class OverwriteBenchmarks
{
    StringBuilder smallBuilder = null!;
    StringBuilder mediumBuilder = null!;
    StringBuilder largeBuilder = null!;
    StringBuilder multiChunkBuilder = null!;

    [GlobalSetup]
    public void Setup()
    {
        smallBuilder = new StringBuilder("Hello World! This is a small test string.");

        mediumBuilder = new StringBuilder();
        for (var i = 0; i < 100; i++)
        {
            mediumBuilder.Append("This is sentence number ");
            mediumBuilder.Append(i);
            mediumBuilder.Append(". ");
        }

        largeBuilder = new StringBuilder();
        for (var i = 0; i < 10000; i++)
        {
            largeBuilder.Append("Content ");
        }

        // Force multiple chunks
        multiChunkBuilder = new StringBuilder();
        for (var i = 0; i < 50; i++)
        {
            multiChunkBuilder.Append("Chunk ");
            multiChunkBuilder.Append(i);
            multiChunkBuilder.Append(" data. ");
        }
    }

    // Same length replacements - most common case
    [Benchmark]
    public void SameLength_Small_SingleChar()
    {
        var builder = new StringBuilder(smallBuilder.ToString());
        builder.Overwrite("X", 0, 1);
    }

    [Benchmark]
    public void SameLength_Small_FiveChars()
    {
        var builder = new StringBuilder(smallBuilder.ToString());
        builder.Overwrite("XXXXX", 6, 5);
    }

    [Benchmark]
    public void SameLength_Small_TenChars()
    {
        var builder = new StringBuilder(smallBuilder.ToString());
        builder.Overwrite("XXXXXXXXXX", 10, 10);
    }

    [Benchmark]
    public void SameLength_Medium_TenChars()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        builder.Overwrite("REPLACEMENT", 500, 11);
    }

    [Benchmark]
    public void SameLength_Large_TenChars()
    {
        var builder = new StringBuilder(largeBuilder.ToString());
        builder.Overwrite("REPLACEMENT", 5000, 11);
    }

    [Benchmark]
    public void SameLength_MultiChunk_SpanningChunks()
    {
        var builder = new StringBuilder(multiChunkBuilder.ToString());
        builder.Overwrite("REPLACED", 50, 8);
    }

    // Shorter replacements
    [Benchmark]
    public void Shorter_Small_ReplaceWithEmpty()
    {
        var builder = new StringBuilder(smallBuilder.ToString());
        builder.Overwrite("", 6, 5);
    }

    [Benchmark]
    public void Shorter_Small_ReplaceWithOne()
    {
        var builder = new StringBuilder(smallBuilder.ToString());
        builder.Overwrite("X", 6, 5);
    }

    [Benchmark]
    public void Shorter_Medium()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        builder.Overwrite("SHORT", 500, 20);
    }

    [Benchmark]
    public void Shorter_Large()
    {
        var builder = new StringBuilder(largeBuilder.ToString());
        builder.Overwrite("X", 5000, 100);
    }

    // Longer replacements
    [Benchmark]
    public void Longer_Small()
    {
        var builder = new StringBuilder(smallBuilder.ToString());
        builder.Overwrite("MUCH LONGER REPLACEMENT", 6, 5);
    }

    [Benchmark]
    public void Longer_Medium()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        builder.Overwrite("THIS IS A MUCH LONGER REPLACEMENT STRING", 500, 10);
    }

    [Benchmark]
    public void Longer_Large()
    {
        var builder = new StringBuilder(largeBuilder.ToString());
        builder.Overwrite("VERY LONG REPLACEMENT TEXT HERE", 5000, 10);
    }

    // Position-based tests
    [Benchmark]
    public void SameLength_AtStart()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        builder.Overwrite("REPLACED", 0, 8);
    }

    [Benchmark]
    public void SameLength_AtEnd()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        var length = builder.Length;
        builder.Overwrite("REPLACED", length - 8, 8);
    }

    [Benchmark]
    public void SameLength_InMiddle()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        var length = builder.Length;
        builder.Overwrite("REPLACED", length / 2, 8);
    }

    // Multiple operations
    [Benchmark]
    public void MultipleOverwrites_SameLength()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        for (var i = 0; i < 10; i++)
        {
            builder.Overwrite("REPLACE", i * 100, 7);
        }
    }

    [Benchmark]
    public void MultipleOverwrites_DifferentLengths()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        builder.Overwrite("SHORT", 100, 10);
        builder.Overwrite("MUCH LONGER", 200, 5);
        builder.Overwrite("SAME", 300, 4);
    }

    // Edge cases
    [Benchmark]
    public void EdgeCase_VeryLongReplacement()
    {
        var builder = new StringBuilder(mediumBuilder.ToString());
        var longString = new string('X', 500);
        builder.Overwrite(longString, 100, 50);
    }

    [Benchmark]
    public void EdgeCase_SingleCharReplace()
    {
        var builder = new StringBuilder(largeBuilder.ToString());
        builder.Overwrite("Y", 5000, 1);
    }

    [Benchmark]
    public void EdgeCase_NearChunkBoundary()
    {
        var builder = new StringBuilder(multiChunkBuilder.ToString());
        // Try to hit near chunk boundaries
        builder.Overwrite("BOUNDARY", 15, 8);
    }
}