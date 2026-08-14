using DiffEngine;

// Lives here, rather than in Verify.Tests, since the queue seam and BuildServerDetector are static,
// and this project runs serially with BaseTest resetting them between tests.
//
// Queueing is the inline equivalent of launching a diff tool, so it answers to the same switches.
// Without that, a failing test that asked for no diff tooling still piles patches into the viewer,
// and those patches point at real source that an accept would rewrite.
[SuppressMessage("Performance", "CA1857:A constant is expected for the parameter")]
public class InlineQueueTests :
    BaseTest,
    IDisposable
{
    List<InlinePatch> queued = [];
    Func<InlinePatch, Task<InlineResult>> originalAddInline = InlineEngine.AddInline;
    bool originalDisabled = DiffRunner.Disabled;

    public InlineQueueTests()
    {
        InlineEngine.AddInline = patch =>
        {
            queued.Add(patch);
            return Task.FromResult(InlineResult.Queued);
        };

        // The ambient values feed diffEnabled, so pin both here and let each test move the one it
        // owns. Otherwise these pass or fail based on whether the run is on a build server.
        BuildServerDetector.Detected = false;
        DiffRunner.Disabled = false;
    }

    // BaseTest restores BuildServerDetector.Detected for every test, so only the seam and
    // DiffRunner need undoing
    public void Dispose()
    {
        InlineEngine.AddInline = originalAddInline;
        DiffRunner.Disabled = originalDisabled;
    }

    /// <summary>
    /// The counterweight to the tests below: without this they would still pass if the seam stopped
    /// being reached at all.
    /// </summary>
    [Fact]
    public async Task QueuedWhenDiffEnabled()
    {
        using var temp = new TempDirectory();

        await Assert.ThrowsAsync<VerifyException>(() => Verify("value", Settings(temp)));

        var patch = Assert.Single(queued);
        Assert.Equal("value", patch.NewContent);
        Assert.Equal(1, patch.LineHint);
    }

    /// <summary>
    /// The viewer labels and groups queue entries by this, and falls back to the bare call site
    /// without it, so an unnamed patch is a queue that cannot group.
    /// </summary>
    [Fact]
    public async Task QueuedPatchCarriesTheTestName()
    {
        using var temp = new TempDirectory();

        await Assert.ThrowsAsync<VerifyException>(() => Verify("value", Settings(temp)));

        var patch = Assert.Single(queued);
        Assert.Equal($"{nameof(InlineQueueTests)}.{nameof(QueuedPatchCarriesTheTestName)}", patch.TestName);
    }

    [Fact]
    public async Task NotQueuedWhenDiffDisabled()
    {
        using var temp = new TempDirectory();
        var settings = Settings(temp);
        settings.DisableDiff();

        await Assert.ThrowsAsync<VerifyException>(() => Verify("value", settings));

        Assert.Empty(queued);
    }

    [Fact]
    public async Task NotQueuedOnBuildServer()
    {
        using var temp = new TempDirectory();
        var settings = Settings(temp);
        BuildServerDetector.Detected = true;

        await Assert.ThrowsAsync<VerifyException>(() => Verify("value", settings));

        Assert.Empty(queued);
    }

    /// <summary>
    /// DiffEngine already declines a disabled runner, so this pins the near side of that contract
    /// rather than relying on the far side to keep holding it.
    /// </summary>
    [Fact]
    public async Task NotQueuedWhenDiffRunnerDisabled()
    {
        using var temp = new TempDirectory();
        var settings = Settings(temp);
        DiffRunner.Disabled = true;

        await Assert.ThrowsAsync<VerifyException>(() => Verify("value", settings));

        Assert.Empty(queued);
    }

    static VerifySettings Settings(TempDirectory temp)
    {
        var settings = new VerifySettings();
        settings.UseDirectory(temp);
        // Deliberately not this file: the verify below fails and stages a patch, and pointing it at
        // real source would let a tray accept rewrite it
        settings.Snapshot("wrong", temp.BuildPath("Fake.cs"), 1, "\"wrong\"");
        return settings;
    }
}
