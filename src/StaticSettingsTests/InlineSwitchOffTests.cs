using DiffEngine;

// Lives here, rather than in Verify.Tests, since the inline switch is a static setting. This
// project runs serially with BaseTest resetting it between tests, and resetting it part way through
// a test is what stands in for the next run, with the switch taken out of the module initializer.
//
// A snapshot the switch inlines has no Snapshot call yet, so the patch it queues appends one. Once
// the switch is off no verification knows that call site was ever inline, so without a record of it
// the entry stayed pending for good, and accepting it turned inline back on for that test.
public class InlineSwitchOffTests :
    BaseTest,
    IDisposable
{
    List<InlinePatch> queued = [];
    List<Retired> retired = [];
    Func<InlinePatch, Task<InlineResult>> originalAddInline = InlineEngine.AddInline;
    Action<string, int, string?> originalSendRetire = InlineEngine.SendRetire;
    bool originalDisabled = DiffRunner.Disabled;
    TempDirectory temp = new();

    public InlineSwitchOffTests()
    {
        // Both stood in for: the call sites are real ones in this file, and whatever owns the queue
        // on this machine would otherwise be sent them
        InlineEngine.AddInline = _ =>
        {
            queued.Add(_);
            return Task.FromResult(InlineResult.Queued);
        };
        InlineEngine.SendRetire = Retire;

        // The ambient values feed diffEnabled, and a retire answers to the same switches, so pin
        // both rather than pass or fail by whether the run is on a build server or under an AI CLI
        BuildServerDetector.Detected = false;
        DiffRunner.Disabled = false;
    }

    void Retire(string sourceFile, int line, string? memberName) =>
        retired.Add(new(sourceFile, line, memberName));

    // BaseTest restores BuildServerDetector.Detected and deletes the records for every test, so only
    // the seams and DiffRunner need undoing
    public void Dispose()
    {
        InlineEngine.AddInline = originalAddInline;
        InlineEngine.SendRetire = originalSendRetire;
        DiffRunner.Disabled = originalDisabled;
        temp.Dispose();
    }

    [Fact]
    public async Task SwitchingOffRetiresWhatTheSwitchQueued()
    {
        VerifierSettings.Inline();
        await Assert.ThrowsAsync<VerifyException>(() => Verify("value"));
        var patch = Assert.Single(queued);
        Assert.Equal(InlinePatchMode.Append, patch.Mode);
        Assert.Single(Records());

        // The next run, with the switch gone from the module initializer
        VerifierSettings.Reset();
        await Verify("value", PassingSettings());

        var retire = Assert.Single(retired);
        Assert.Equal(patch.SourceFile, retire.SourceFile);
        Assert.Equal(patch.LineHint, retire.Line);
        // The record holds the line the entry was queued under, which is the key the owner holds it
        // by. So no member, which is what could reach a sibling call site's entry instead
        Assert.Null(retire.MemberName);
        Assert.Empty(Records());
    }

    /// <summary>
    /// A queued snapshot does not always stay in the queue: an owner on its way out writes what it
    /// holds under the source project's obj, which accept tooling reads just as it reads the queue.
    /// So the retire has to reach those files as well.
    /// </summary>
    [Fact]
    public async Task SwitchingOffClearsWhatTheSwitchLeftOnDisk()
    {
        VerifierSettings.Inline();
        await Assert.ThrowsAsync<VerifyException>(() => Verify("value"));
        var patch = Assert.Single(queued);

        var staging = Path.Combine(
            AttributeReader.GetProjectDirectory(typeof(InlineSwitchOffTests).Assembly),
            "obj",
            InlineStaging.DirectoryName);
        Directory.CreateDirectory(staging);
        var stem = nameof(SwitchingOffClearsWhatTheSwitchLeftOnDisk);
        var patchFile = Path.Combine(staging, $"{stem}.inlinepatch");
        var receivedFile = Path.Combine(staging, $"{stem}.received.txt");
        var expectedFile = Path.Combine(staging, $"{stem}.expected.txt");
        InlinePatchFile.Write(patchFile, patch);
        await File.WriteAllTextAsync(receivedFile, patch.NewContent);
        await File.WriteAllTextAsync(expectedFile, "");

        VerifierSettings.Reset();
        await Verify("value", PassingSettings());

        Assert.False(File.Exists(patchFile));
        Assert.False(File.Exists(receivedFile));
        Assert.False(File.Exists(expectedFile));
    }

    /// <summary>
    /// While the switch is on, what it queued is pending for a reason: the call site is still an
    /// inline snapshot, waiting to be accepted.
    /// </summary>
    [Fact]
    public async Task SwitchStillOnKeepsTheRecord()
    {
        VerifierSettings.Inline();
        await Assert.ThrowsAsync<VerifyException>(() => Verify("value"));
        var patch = Assert.Single(queued);

        // The next run, with the switch still on. NotInline so this verification passes as a file,
        // which retires its own call site and nothing else
        VerifierSettings.Reset();
        VerifierSettings.Inline();
        var settings = PassingSettings();
        settings.NotInline();
        await Verify("value", settings);

        Assert.DoesNotContain(retired, _ => _.Line == patch.LineHint);
        Assert.Single(Records());
    }

    /// <summary>
    /// Every verification of a codebase that never turned the switch on comes through here, so with
    /// nothing recorded nothing may reach the queue owner.
    /// </summary>
    [Fact]
    public async Task NothingRecordedRetiresNothing()
    {
        await Verify("value", PassingSettings());

        Assert.Empty(retired);
    }

    /// <summary>
    /// A verification that may not reach the queue owner leaves the records for one that can,
    /// rather than dropping them unretired.
    /// </summary>
    [Fact]
    public async Task DiffDisabledLeavesTheRecordsForALaterVerification()
    {
        VerifierSettings.Inline();
        await Assert.ThrowsAsync<VerifyException>(() => Verify("value"));
        var patch = Assert.Single(queued);

        VerifierSettings.Reset();
        var disabled = PassingSettings();
        disabled.DisableDiff();
        await Verify("value", disabled);

        Assert.Empty(retired);
        Assert.Single(Records());

        await Verify("value", PassingSettings());

        Assert.Equal(patch.LineHint, Assert.Single(retired).Line);
        Assert.Empty(Records());
    }

    /// <summary>
    /// Only the switch appends. A patch that sets an argument belongs to a Snapshot call the source
    /// still holds, and that call site settles or retires itself.
    /// </summary>
    [Fact]
    public async Task AnExplicitSnapshotIsNotRecorded()
    {
        var settings = new VerifySettings();
        settings.UseDirectory(temp);
        // Deliberately not this file, as in InlineQueueTests
        settings.Snapshot("wrong", temp.BuildPath("Fake.cs"), 1, "\"wrong\"");

        await Assert.ThrowsAsync<VerifyException>(() => Verify("value", settings));

        Assert.Equal(InlinePatchMode.Set, Assert.Single(queued).Mode);
        Assert.Empty(Records());
    }

    // A file verification that passes. A failing one hands a pending move to whatever owns the queue
    // on this machine, so AutoVerify accepts it into the temp directory instead
    VerifySettings PassingSettings()
    {
        var settings = new VerifySettings();
        settings.UseDirectory(temp);
        settings.AutoVerify();
        settings.DisableRequireUniquePrefix();
        return settings;
    }

    static List<string> Records() =>
        Directory.Exists(InlineSwitchRecordsDirectory)
            ? Directory.EnumerateFiles(InlineSwitchRecordsDirectory).ToList()
            : [];

    sealed record Retired(string SourceFile, int Line, string? MemberName);
}
