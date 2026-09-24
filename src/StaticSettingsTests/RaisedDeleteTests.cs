using DiffEngine;

// Lives here, rather than in Verify.Tests, since what a run raised is read back once per process,
// and resetting that part way through a test is what stands in for the next run. This project
// runs serially with BaseTest resetting between tests.
//
// A delete is raised for a verified file no target produced, and waits in the tray or the viewer
// for someone to accept it. Once a later run verifies against that file again, accepting the delete
// would remove a file a passing test depends on, so the run that finds it in use withdraws it.
public class RaisedDeleteTests :
    BaseTest,
    IDisposable
{
    List<string> added = [];
    List<string> settled = [];
    Func<string, Task> originalAddDelete = RaisedDeletes.AddDelete;
    Action<string> originalSettleDelete = RaisedDeletes.SettleDelete;
    bool originalDisabled = DiffRunner.Disabled;
    TempDirectory temp = new();

    public RaisedDeleteTests()
    {
        // Both stood in for, so nothing reaches whatever tray or viewer is running on this machine
        RaisedDeletes.AddDelete = _ =>
        {
            added.Add(_);
            return Task.CompletedTask;
        };
        RaisedDeletes.SettleDelete = _ => settled.Add(_);

        // DiffEngine switches itself off on a build server, under continuous testing and under an
        // AI CLI, and both a delete and its settle answer to that switch
        DiffRunner.Disabled = false;
    }

    public void Dispose()
    {
        RaisedDeletes.AddDelete = originalAddDelete;
        RaisedDeletes.SettleDelete = originalSettleDelete;
        DiffRunner.Disabled = originalDisabled;
        temp.Dispose();
    }

    [Fact]
    public async Task ADeleteIsWithdrawnOnceItsFileIsInUseAgain()
    {
        var (first, second) = await SeedTwoTargets();

        // One target, so both indexed files are left over from a run that produced two
        await Assert.ThrowsAsync<VerifyException>(() => Verify("a", Settings()));
        Assert.Equal([first, second], added.Order(StringComparer.Ordinal));
        Assert.Equal(2, Records().Count);
        Assert.Empty(settled);

        // The next run, with the second target back
        VerifierSettings.Reset();
        await Verify(TwoTargets("a", "b"), Settings());

        Assert.Equal([first, second], settled.Order(StringComparer.Ordinal));
        Assert.Empty(Records());
        Assert.True(File.Exists(first));
        Assert.True(File.Exists(second));
    }

    /// <summary>
    /// A file that no longer matches is still in use: the mismatch is a pending move onto it, and
    /// accepting a delete of it as well would leave that move with nothing to replace.
    /// </summary>
    [Fact]
    public async Task ADeleteIsWithdrawnWhenItsFileNoLongerMatches()
    {
        var (first, second) = await SeedTwoTargets();
        await Assert.ThrowsAsync<VerifyException>(() => Verify("a", Settings()));

        VerifierSettings.Reset();
        await Assert.ThrowsAsync<VerifyException>(() => Verify(TwoTargets("a", "changed"), Settings()));

        Assert.Equal([first, second], settled.Order(StringComparer.Ordinal));
        Assert.Empty(Records());
    }

    /// <summary>
    /// A run that cannot reach the owner leaves the records for one that can, rather than dropping
    /// them with their deletes still pending.
    /// </summary>
    [Fact]
    public async Task WithDiffEngineOffTheRecordsWaitForALaterRun()
    {
        var (first, second) = await SeedTwoTargets();
        await Assert.ThrowsAsync<VerifyException>(() => Verify("a", Settings()));

        VerifierSettings.Reset();
        DiffRunner.Disabled = true;
        await Verify(TwoTargets("a", "b"), Settings());

        Assert.Empty(settled);
        Assert.Equal(2, Records().Count);

        VerifierSettings.Reset();
        DiffRunner.Disabled = false;
        await Verify(TwoTargets("a", "b"), Settings());

        Assert.Equal([first, second], settled.Order(StringComparer.Ordinal));
        Assert.Empty(Records());
    }

    /// <summary>
    /// Accepting a delete removes its file, and a tray drops a delete whose file is missing, so the
    /// record has nothing left to withdraw. The next run drops it rather than keeping it for good.
    /// </summary>
    [Fact]
    public async Task ARecordWhoseFileHasGoneIsDropped()
    {
        var (first, second) = await SeedTwoTargets();
        await Assert.ThrowsAsync<VerifyException>(() => Verify("a", Settings()));
        Assert.Equal(2, Records().Count);

        // Both deletes accepted
        File.Delete(first);
        File.Delete(second);

        VerifierSettings.Reset();
        await Assert.ThrowsAsync<VerifyException>(() => Verify("a", Settings()));

        Assert.Empty(Records());
        Assert.Empty(settled);
    }

    /// <summary>
    /// Where DiffEngine is off no delete is raised, so there is nothing to withdraw later.
    /// </summary>
    [Fact]
    public async Task NothingIsRecordedWhileDiffEngineIsOff()
    {
        await SeedTwoTargets();
        DiffRunner.Disabled = true;

        await Assert.ThrowsAsync<VerifyException>(() => Verify("a", Settings()));

        Assert.Empty(Records());
    }

    /// <summary>
    /// Every verification of every codebase comes through here, so with nothing raised nothing may
    /// reach the queue owner.
    /// </summary>
    [Fact]
    public async Task NothingRaisedSettlesNothing()
    {
        await File.WriteAllTextAsync(Path.Combine(temp.Path, "Shared.verified.txt"), "a");

        await Verify("a", Settings());

        Assert.Empty(added);
        Assert.Empty(settled);
    }

    // Two targets with the same extension, which is what indexes their names
    async Task<(string First, string Second)> SeedTwoTargets()
    {
        var first = Path.Combine(temp.Path, "Shared#00.verified.txt");
        var second = Path.Combine(temp.Path, "Shared#01.verified.txt");
        await File.WriteAllTextAsync(first, "a");
        await File.WriteAllTextAsync(second, "b");
        return (first, second);
    }

    static List<Target> TwoTargets(string first, string second) =>
    [
        new("txt", first),
        new("txt", second)
    ];

    VerifySettings Settings()
    {
        var settings = new VerifySettings();
        settings.UseDirectory(temp);
        settings.UseFileName("Shared");
        // No diff tool is launched and no pending move is sent, so the delete and its settle are
        // all that reach DiffEngine, and both are stood in for
        settings.DisableDiff();
        settings.DisableRequireUniquePrefix();
        return settings;
    }

    // Filtered to this test's files, so a record another test left behind cannot fail this one
    List<string> Records()
    {
        var directory = Path.Combine(
            AttributeReader.GetIntermediateDirectory(typeof(RaisedDeleteTests).Assembly),
            RaisedDeletes.DirectoryName);
        if (!Directory.Exists(directory))
        {
            return [];
        }

        return Directory.EnumerateFiles(directory)
            .Select(File.ReadAllText)
            .Where(_ => _.StartsWith(temp.Path, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
