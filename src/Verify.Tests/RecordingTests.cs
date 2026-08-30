public class RecordingTests
{
    [Fact]
    public Task IgnoreNames()
    {
        Recording.IgnoreNames("ignore");
        Recording.Start();
        Recording.Add("ignore", "value1");
        Recording.Add("name", "value2");
        return Verify();
    }

    [Fact]
    public async Task StoppedInChildContextIsNotRecording()
    {
        Recording.Start();
        Recording.Add("name", "value");
        // Consuming inside a child execution context (as the verify engine does)
        // must not leave the recording active in this context.
        await Task.Run(() => Recording.TryStop(out _));
        Assert.False(Recording.IsRecording());
    }

    // The engine consumes the recording in a child context, so the null does not flow back
    // and the test's own context still holds the stopped state. Starting again after that
    // is the supported pattern, not a double start.
    [Fact]
    public async Task StartAfterConsumedInChildContext()
    {
        Recording.Start();
        Recording.Add("name", "value1");
        await Task.Run(() => Recording.TryStop(out _));

        Recording.Start();
        Recording.Add("name", "value2");

        Assert.True(Recording.TryStop(out var recorded));
        Assert.Equal(["value2"], recorded.Select(_ => _.Data));
    }

    [Fact]
    public void StartWhileRecordingStillThrows()
    {
        using (Recording.Start())
        {
            var exception = Assert.Throws<Exception>(Recording.Start);
            Assert.Equal("Recording already started", exception.Message);
        }
    }

    [Fact]
    public void DisposeAfterStopDoesNotThrow()
    {
        using (Recording.Start())
        {
            Recording.Add("name", "value");
            Recording.Stop();
        }
    }

    [Fact]
    public void DisposeNamedAfterStopDoesNotThrow()
    {
        using (Recording.Start("DisposeNamedAfterStop"))
        {
            Recording.Add("DisposeNamedAfterStop", "name", "value");
            Recording.Stop("DisposeNamedAfterStop");
        }
    }

    // Named state lives in a static dictionary, so a scope left without an explicit Stop,
    // for example because the test threw, would otherwise hold it and its recorded objects
    // forever, and block the identifier for every later Start.
    [Fact]
    public void DisposeNamedReleasesTheIdentifier()
    {
        try
        {
            using (Recording.Start("DisposeNamedReleases"))
            {
                Recording.Add("DisposeNamedReleases", "name", "value");
                throw new("test failure");
            }
        }
        catch (Exception exception)
            when (exception.Message == "test failure")
        {
        }

        Assert.False(Recording.IsRecording("DisposeNamedReleases"));

        // The identifier is free again
        using (Recording.Start("DisposeNamedReleases"))
        {
            Recording.Add("DisposeNamedReleases", "name", "value2");
            var recorded = Recording.Stop("DisposeNamedReleases");
            Assert.Equal(["value2"], recorded.Select(_ => _.Data));
        }
    }

    [Fact]
    public Task Dates()
    {
        Recording.Start();
        var dateTime = DateTime.Now;
        Recording.Add("typed", dateTime);
        Recording.Add("inline", $"a {dateTime:F} b");
        return Verify().ScrubInlineDateTimes("F");
    }

    #region Recording

    [Fact]
    public Task Usage()
    {
        Recording.Start();
        Recording.Add("name", "value");
        return Verify("TheValue");
    }

    #endregion

    [Fact]
    public Task Ignore()
    {
        Recording.Start();
        Recording.Add("name1", "value1");
        Recording.Add("name1", "value11");
        Recording.Add("name2", "value2");
        Recording.Add("name2", "value3");
        return Verify("TheValue")
            .IgnoreMember("name1");
    }

    [Fact]
    public Task Scrub()
    {
        Recording.Start();
        Recording.Add("name1", "value1");
        Recording.Add("name1", "value11");
        Recording.Add("name2", "value2");
        Recording.Add("name2", "value3");
        return Verify("TheValue")
            .ScrubMember("name1");
    }

    #region RecordingTryAdd

    [Fact]
    public Task TryAdd()
    {
        //using Recording.Add here would throw since Recording.Start has not been called
        Recording.TryAdd("name1", "value1");
        Recording.Start();
        Recording.TryAdd("name2", "value2");
        return Verify("TheValue");
    }

    #endregion

    [Fact]
    public void NameExists()
    {
        Recording.Start();
        Assert.False(Recording.NameExists("name"));
        Recording.Add("name", "value");
        Assert.True(Recording.NameExists("name"));
    }

    #region RecordingScoped

    [Fact]
    public Task RecordingScoped()
    {
        using (Recording.Start())
        {
            Recording.Add("name1", "value1");
        }

        // Recording.Add is ignored here
        Recording.Add("name2", "value2");
        return Verify();
    }

    #endregion

    #region RecordingIdentifier

    [Fact]
    public Task Identifier()
    {
        Recording.Start("identifier");
        Recording.Add("identifier", "name", "value");
        return Verify(Recording.Stop("identifier"));
    }

    #endregion

    #region IsRecording

    [Fact]
    public void IsRecording()
    {
        Assert.False(Recording.IsRecording());
        Recording.Start();
        Assert.True(Recording.IsRecording());
    }

    #endregion

    [Fact]
    public void IsRecordingIdentifier()
    {
        Assert.False(Recording.IsRecording("IsRecordingIdentifier"));
        Recording.Start("IsRecordingIdentifier");
        Assert.True(Recording.IsRecording("IsRecordingIdentifier"));
    }

    [Fact]
    public Task NoRecording() =>
        Throws(() => Recording.Add("name", "value"))
            .IgnoreStackTrace();

    [Fact]
    public Task NoRecordingIdentifier() =>
        Throws(() => Recording.Add("NoRecordingIdentifier", "name", "value"))
            .IgnoreStackTrace();

    #region RecordingStop

    [Fact]
    public Task Stop()
    {
        Recording.Start();
        Recording.Add("name1", "value1");
        Recording.Add("name2", "value2");
        var appends = Recording.Stop();
        return Verify(appends.Where(_ => _.Name != "name1"));
    }

    #endregion

    #region RecordingStopNotInResult

    [Fact]
    public Task StopNotInResult()
    {
        Recording.Start();
        Recording.Add("name1", "value1");
        Recording.Add("name2", "value2");
        Recording.Stop();
        return Verify("other data");
    }

    #endregion

    [Fact]
    public Task StopIdentifier()
    {
        Recording.Start("StopIdentifier");
        Recording.Add("StopIdentifier", "name", "value");
        return Verify(Recording.Stop("StopIdentifier"));
    }

    [Fact]
    public Task TryStopNoStart()
    {
        var result = Recording.TryStop(out var recorded);
        return Verify(
            new
            {
                result,
                recorded
            });
    }

    [Fact]
    public Task TryStopNoStartIdentifier()
    {
        var result = Recording.TryStop("TryStopNoStartIdentifier", out var recorded);
        return Verify(
            new
            {
                result,
                recorded
            });
    }

    #region RecordingClear

    [Fact]
    public Task Clear()
    {
        Recording.Start();
        Recording.Add("name1", "value1");
        Recording.Clear();
        Recording.Add("name2", "value2");
        return Verify();
    }

    #endregion

    [Fact]
    public Task ClearIdentifier()
    {
        Recording.Start("ClearIdentifier");
        Recording.Add("ClearIdentifier", "name1", "value");
        Recording.Clear("ClearIdentifier");
        Recording.Add("ClearIdentifier", "name2", "value");
        return Verify(Recording.Stop("ClearIdentifier"));
    }

    #region RecordingPauseResume

    [Fact]
    public Task PauseResume()
    {
        Recording.Start();
        Recording.Pause();
        Recording.Add("name1", "value1");
        Recording.Resume();
        Recording.Add("name2", "value2");
        Recording.Pause();
        Recording.Add("name3", "value3");
        return Verify();
    }

    #endregion

    [Fact]
    public Task PauseResumeIdentifier()
    {
        Recording.Start("PauseResumeIdentifier");
        Recording.Pause("PauseResumeIdentifier");
        Recording.Add("PauseResumeIdentifier", "name1", "value");
        Recording.Resume("PauseResumeIdentifier");
        Recording.Add("PauseResumeIdentifier", "name2", "value");
        Recording.Pause("PauseResumeIdentifier");
        Recording.Add("PauseResumeIdentifier", "name3", "value");
        return Verify(Recording.Stop("PauseResumeIdentifier"));
    }

    [Fact]
    public Task NoValue()
    {
        Recording.Start();
        Recording.Add("name", "value");
        return Verify();
    }

    [Fact]
    public Task Multiple()
    {
        Recording.Start();
        Recording.Add("name1", "value1");
        Recording.Add("name2", "value2");
        return Verify("TheValue");
    }

    [Fact]
    public Task MultipleIdentifier()
    {
        Recording.Start("MultipleIdentifier");
        Recording.Add("MultipleIdentifier", "name1", "value1");
        Recording.Add("MultipleIdentifier", "name2", "value2");
        return Verify(Recording.Stop("MultipleIdentifier"));
    }

    #region RecordingSameKey

    [Fact]
    public Task SameKey()
    {
        Recording.Start();
        Recording.Add("name", "value1");
        Recording.Add("name", "value2");
        return Verify("TheValue");
    }

    #endregion

    [Fact]
    public Task AppendIdentifier()
    {
        Recording.Start("AppendIdentifier");
        Recording.Add("AppendIdentifier", "name", "value1");
        Recording.Add("AppendIdentifier", "name", "value2");
        return Verify(Recording.Stop("AppendIdentifier"));
    }

    #region RecordingIgnoreCase

    [Fact]
    public Task Case()
    {
        Recording.Start();
        Recording.Add("name", "value1");
        Recording.Add("Name", "value2");
        return Verify("TheValue");
    }

    #endregion

    [Fact]
    public Task ToDictionary()
    {
        Recording.Start();
        Recording.Add("name", "value1");
        Recording.Add("Name", "value2");
        var appends = Recording.Stop();
        var dictionary = appends.ToDictionary();
        return Verify(dictionary);
    }

    [Fact]
    public Task CaseIdentifier()
    {
        Recording.Start("CaseIdentifier");
        Recording.Add("CaseIdentifier", "name", "value1");
        Recording.Add("CaseIdentifier", "Name", "value2");
        return Verify(Recording.Stop("CaseIdentifier"));
    }
}