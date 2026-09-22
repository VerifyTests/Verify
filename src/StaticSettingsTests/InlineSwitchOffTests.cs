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
    /// A test deleted while its snapshot was pending leaves nothing to verify at its call site, so
    /// the record is all that still names the entry. The retire does not consult the source, and
    /// must not start to: a verify call at the recorded line is exactly what a deleted test lacks.
    /// </summary>
    [Fact]
    public async Task ARecordWhoseTestWasDeletedIsStillRetired()
    {
        // The file after the delete, with no verify call left at the recorded line
        var source = temp.BuildPath("Tests.cs");
        await File.WriteAllTextAsync(
            source,
            """
            class Tests
            {
                [Fact]
                public Task Remaining() =>
                    Task.CompletedTask;
            }
            """);
        InlineSwitchRecords.Write(source, 5);

        await Verify("value", PassingSettings());

        var retire = Assert.Single(retired);
        Assert.Equal(source, retire.SourceFile);
        Assert.Equal(5, retire.Line);
        Assert.Empty(Records());
    }

    /// <summary>
    /// A switched-off run retires before its first verification can queue anything, so a record
    /// cannot drop an entry that run has just queued under the same key: here an explicit Snapshot
    /// call that now sits on the recorded line, failing against its literal.
    /// </summary>
    [Fact]
    public async Task ARecordCannotRetireWhatTheSwitchedOffRunQueues()
    {
        var events = new List<string>();
        InlinePatch? appended = null;
        InlineEngine.AddInline = patch =>
        {
            appended ??= patch;
            events.Add($"queue {patch.LineHint}");
            return Task.FromResult(InlineResult.Queued);
        };
        InlineEngine.SendRetire = (_, line, _) => events.Add($"retire {line}");

        // One call site for both runs, since the record is keyed by the line
        for (var run = 0; run < 2; run++)
        {
            VerifierSettings.Reset();
            var settings = new VerifySettings();
            if (run == 0)
            {
                VerifierSettings.Inline();
            }
            else
            {
                // The next run, switched off. This file and line, so the patch is queued under the
                // key the record names, which the seams keep from whatever owns the queue here
                settings.Snapshot("old", appended!.SourceFile, appended.LineHint, "\"old\"");
            }

            await Assert.ThrowsAsync<VerifyException>(() => Verify("value", settings));
        }

        var line = appended!.LineHint;
        Assert.Equal([$"queue {line}", $"retire {line}", $"queue {line}"], events);
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
    /// As <see cref="DiffDisabledLeavesTheRecordsForALaterVerification" />, with diff off for the
    /// whole process, which DiffEngine does itself under continuous testing and an AI CLI. The
    /// settings are built first, since they read the same switch, and this is about the check that
    /// reads it directly.
    /// </summary>
    [Fact]
    public async Task DiffRunnerDisabledLeavesTheRecordsForALaterVerification()
    {
        VerifierSettings.Inline();
        await Assert.ThrowsAsync<VerifyException>(() => Verify("value"));
        var patch = Assert.Single(queued);

        VerifierSettings.Reset();
        var settings = PassingSettings();
        DiffRunner.Disabled = true;
        await Verify("value", settings);

        Assert.Empty(retired);
        Assert.Single(Records());

        DiffRunner.Disabled = false;
        await Verify("value", PassingSettings());

        Assert.Equal(patch.LineHint, Assert.Single(retired).Line);
        Assert.Empty(Records());
    }

    /// <summary>
    /// As <see cref="DiffDisabledLeavesTheRecordsForALaterVerification" />, on a build server.
    /// </summary>
    [Fact]
    public async Task ABuildServerLeavesTheRecordsForALaterVerification()
    {
        VerifierSettings.Inline();
        await Assert.ThrowsAsync<VerifyException>(() => Verify("value"));
        var patch = Assert.Single(queued);

        VerifierSettings.Reset();
        BuildServerDetector.Detected = true;
        await Verify("value", PassingSettings());

        Assert.Empty(retired);
        Assert.Single(Records());

        BuildServerDetector.Detected = false;
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

    /// <summary>
    /// A record lives only while its call site is one the switch appends to. Once a switched-on
    /// verification at that call site is not appended to any more, the record would only ever
    /// retire whatever sits on that line at switch off, which can be an explicit Snapshot call's
    /// own pending snapshot.
    /// </summary>
    [Fact]
    public async Task ACallSiteTheSwitchNoLongerAppendsToForgetsItsRecord()
    {
        // One call site for both runs, since the record is keyed by the line
        for (var run = 0; run < 2; run++)
        {
            VerifierSettings.Reset();
            VerifierSettings.Inline();
            VerifySettings? settings = null;
            if (run == 1)
            {
                // The next run, still on, with the call site now declined
                settings = PassingSettings();
                settings.NotInline();
            }

            var verification = Verify("value", settings ?? new());
            if (run == 0)
            {
                await Assert.ThrowsAsync<VerifyException>(() => verification);
                Assert.Single(Records());
                continue;
            }

            await verification;
        }

        Assert.Empty(Records());
    }

    /// <summary>
    /// The case the forget exists for: the snapshot was accepted, so an explicit Snapshot call is
    /// at the call site now, and with the switch still on it is compared rather than appended to.
    /// </summary>
    [Fact]
    public async Task AnAcceptedSnapshotAtTheCallSiteForgetsItsRecord()
    {
        // One call site for both runs, since the record is keyed by the line
        for (var run = 0; run < 2; run++)
        {
            VerifierSettings.Reset();
            VerifierSettings.Inline();
            var settings = new VerifySettings();
            if (run == 1)
            {
                // The next run, still on, with the snapshot accepted. Deliberately not this file, as
                // in AnExplicitSnapshotIsNotRecorded, and diff off so the passing comparison settles
                // nothing with whatever owns the queue on this machine
                settings.Snapshot("value", temp.BuildPath("Fake.cs"), 1, "\"value\"");
                settings.DisableDiff();
            }

            var verification = Verify("value", settings);
            if (run == 0)
            {
                await Assert.ThrowsAsync<VerifyException>(() => verification);
                Assert.Single(Records());
                continue;
            }

            await verification;
        }

        Assert.Empty(Records());
    }

    /// <summary>
    /// A call site the switch stops inlining without NotInline: here its delegate declines it, and
    /// the same goes for a parameter, the size limit or a binary first target. What it queued while
    /// it was inline is stale, so it is retired.
    /// </summary>
    [Fact]
    public async Task ACallSiteTheSwitchDeclinesRetiresWhatItQueued()
    {
        InlinePatch? patch = null;
        // One call site for both runs, since the entry is keyed by the line
        for (var run = 0; run < 2; run++)
        {
            VerifierSettings.Reset();
            VerifySettings? settings = null;
            if (run == 0)
            {
                VerifierSettings.Inline();
            }
            else
            {
                // The next run, still on, with the delegate now declining the call site
                VerifierSettings.Inline((_, _, _, _) => false);
                settings = PassingSettings();
            }

            var verification = Verify("value", settings ?? new());
            if (run == 0)
            {
                await Assert.ThrowsAsync<VerifyException>(() => verification);
                patch = Assert.Single(queued);
                continue;
            }

            await verification;
        }

        var retire = Assert.Single(retired);
        Assert.Equal(patch!.SourceFile, retire.SourceFile);
        Assert.Equal(patch.LineHint, retire.Line);
        Assert.Empty(Records());
    }

    /// <summary>
    /// A record that cannot be read is not a malformed one: deleting it would strand the entry it
    /// names for good, so it is left for a run that can read it.
    /// </summary>
    [Fact]
    public async Task AnUnreadableRecordIsLeftForALaterRun()
    {
        VerifierSettings.Inline();
        await Assert.ThrowsAsync<VerifyException>(() => Verify("value"));
        var record = Assert.Single(Records());

        VerifierSettings.Reset();
        using (new FileStream(record, FileMode.Open, FileAccess.Read, FileShare.Delete))
        {
            await Verify("value", PassingSettings());
        }

        Assert.Empty(retired);
        Assert.Single(Records());
    }

    /// <summary>
    /// A record that reads but names no call site can never be retired, so it is deleted rather
    /// than read again by every later run.
    /// </summary>
    [Fact]
    public async Task AMalformedRecordIsDeleted()
    {
        Directory.CreateDirectory(InlineSwitchRecordsDirectory);
        await File.WriteAllTextAsync(Path.Combine(InlineSwitchRecordsDirectory, "malformed.txt"), "not a record");

        await Verify("value", PassingSettings());

        Assert.Empty(retired);
        Assert.Empty(Records());
    }

    /// <summary>
    /// Cleaning up after the switch must not change a test outcome. A retire that throws costs only
    /// its own record, and the rest are still retired.
    /// </summary>
    [Fact]
    public async Task ARetireThatThrowsDoesNotFailTheVerification()
    {
        InlineSwitchRecords.Write(temp.BuildPath("First.cs"), 1);
        InlineSwitchRecords.Write(temp.BuildPath("Second.cs"), 1);
        var thrown = false;
        InlineEngine.SendRetire = (sourceFile, line, memberName) =>
        {
            if (!thrown)
            {
                thrown = true;
                throw new("The queue owner fell over");
            }

            Retire(sourceFile, line, memberName);
        };

        await Verify("value", PassingSettings());

        Assert.Single(retired);
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
