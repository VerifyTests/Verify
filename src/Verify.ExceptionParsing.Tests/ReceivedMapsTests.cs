using VerifyTests.ExceptionParsing;

// Verify's build server detection is global state, and the contract test below toggles it.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

public class ReceivedMapsTests
{
    // Pins the contract between the writer, in Verify, and the reader here. If either side changes
    // the location or the format, this fails.
    [Fact]
    public async Task ReadsMapWrittenByVerify()
    {
        // Maps are not written on a build server, so enable the writer for this test.
        var detected = DiffEngine.BuildServerDetector.Detected;
        DiffEngine.BuildServerDetector.Detected = false;
        try
        {
            using var temp = new TempDirectory();
            var settings = new VerifySettings();
            settings.UseDirectory(temp);
            settings.DisableDiff();

            await Assert.ThrowsAsync<VerifyException>(() => VerifyXunit.Verifier.Verify("value", settings));

            var received = Directory.EnumerateFiles(temp)
                .Single(_ => _.EndsWith(".received.txt", StringComparison.Ordinal));

            var maps = ReceivedMaps.Read(AttributeReader.GetIntermediateDirectory());

            Assert.True(maps.TryGetVerified(received, out var verified));

            // The received name carries the runtime and version, since this project is multi
            // targeted, while the verified name does not.
            Assert.Equal(
                Path.Combine(temp.Path, "ReceivedMapsTests.ReadsMapWrittenByVerify.verified.txt"),
                verified);
        }
        finally
        {
            DiffEngine.BuildServerDetector.Detected = detected;
        }
    }

    [Fact]
    public void FindsMapsInNestedDirectories()
    {
        using var temp = new TempDirectory();
        var received = Path.Combine(temp.Path, "Foo.received.txt");
        var verified = Path.Combine(temp.Path, "Foo.verified.txt");
        File.WriteAllText(received, "value");
        WriteMap(Path.Combine(temp.Path, "project", "obj", "Debug", "net10.0"), received, verified);

        var maps = ReceivedMaps.Read(temp);

        Assert.Single(maps.Pairs);
        Assert.True(maps.TryGetVerified(received, out var found));
        Assert.Equal(verified, found);
    }

    [Fact]
    public void ExcludesRecordWhenReceivedIsGone()
    {
        using var temp = new TempDirectory();
        // No received file on disk, as after the snapshot was accepted, or the test fixed or deleted.
        var received = Path.Combine(temp.Path, "Foo.received.txt");
        WriteMap(Path.Combine(temp.Path, "obj"), received, Path.Combine(temp.Path, "Foo.verified.txt"));

        var maps = ReceivedMaps.Read(temp);

        // Enumerating Pairs is how a run is accepted, so a record that cannot be acted on must not
        // appear there.
        Assert.Empty(maps.Pairs);
        Assert.False(maps.TryGetVerified(received, out _));
    }

    [Fact]
    public void DeduplicatesReceivedRecordedByMoreThanOneBuild()
    {
        using var temp = new TempDirectory();
        var received = Path.Combine(temp.Path, "Foo.received.txt");
        var verified = Path.Combine(temp.Path, "Foo.verified.txt");
        File.WriteAllText(received, "value");
        WriteMap(Path.Combine(temp.Path, "obj", "Debug"), received, verified);
        WriteMap(Path.Combine(temp.Path, "obj", "Release"), received, verified);

        var maps = ReceivedMaps.Read(temp);

        var pair = Assert.Single(maps.Pairs);
        Assert.Equal(received, pair.Received);
        Assert.Equal(verified, pair.Verified);
    }

    [Fact]
    public void SkipsDirectoriesThatCannotHoldMaps()
    {
        using var temp = new TempDirectory();
        var received = Path.Combine(temp.Path, "Foo.received.txt");
        File.WriteAllText(received, "value");
        WriteMap(Path.Combine(temp.Path, ".git"), received, Path.Combine(temp.Path, "Foo.verified.txt"));

        var maps = ReceivedMaps.Read(temp);

        Assert.Empty(maps.Pairs);
    }

    [Fact]
    public void EmptyWhenDirectoryMissing()
    {
        var maps = ReceivedMaps.Read(Path.Combine(Path.GetTempPath(), $"missing-{Guid.NewGuid()}"));

        Assert.Empty(maps.Pairs);
        Assert.False(maps.TryGetVerified("Foo.received.txt", out _));
    }

    [Fact]
    public void IgnoresMalformedMaps()
    {
        using var temp = new TempDirectory();
        var directory = Path.Combine(temp.Path, "obj", "VerifyReceived");
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, "empty.txt"), "");
        File.WriteAllText(Path.Combine(directory, "single.txt"), "only-a-received-path");
        File.WriteAllLines(Path.Combine(directory, "blank.txt"), ["", ""]);

        var maps = ReceivedMaps.Read(temp);

        Assert.Empty(maps.Pairs);
    }

    [Fact]
    public void LookupNormalizesPaths()
    {
        using var temp = new TempDirectory();
        // recorded with a redundant segment, then looked up with the resolved path
        var recorded = Path.Combine(temp.Path, "sub", "..", "Foo.received.txt");
        var resolved = Path.Combine(temp.Path, "Foo.received.txt");
        var verified = Path.Combine(temp.Path, "Foo.verified.txt");
        File.WriteAllText(resolved, "value");
        WriteMap(Path.Combine(temp.Path, "obj"), recorded, verified);

        var maps = ReceivedMaps.Read(temp);

        Assert.True(maps.TryGetVerified(resolved, out var found));
        Assert.Equal(verified, found);
    }

    static void WriteMap(string parent, string received, string verified)
    {
        var directory = Path.Combine(parent, "VerifyReceived");
        Directory.CreateDirectory(directory);
        File.WriteAllLines(Path.Combine(directory, "map.txt"), [received, verified]);
    }
}
