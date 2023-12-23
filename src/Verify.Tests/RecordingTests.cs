[UsesVerify]
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
        Assert.False(Recording.IsRecording("identifier"));
        Recording.Start("identifier");
        Assert.True(Recording.IsRecording("identifier"));
    }

    [Fact]
    public Task NoRecording() =>
        Throws(() => Recording.Add("name", "value"))
            .IgnoreStackTrace();

    [Fact]
    public Task NoRecordingIdentifier() =>
        Throws(() => Recording.Add("identifier", "name", "value"))
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
        Recording.Start("identifier");
        Recording.Add("identifier", "name", "value");
        return Verify(Recording.Stop("identifier"));
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
        var result = Recording.TryStop("identifier", out var recorded);
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
        Recording.Start("identifier");
        Recording.Add("identifier", "name1", "value");
        Recording.Clear("identifier");
        Recording.Add("identifier", "name2", "value");
        return Verify(Recording.Stop("identifier"));
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
        Recording.Start("identifier");
        Recording.Pause("identifier");
        Recording.Add("identifier", "name1", "value");
        Recording.Resume("identifier");
        Recording.Add("identifier", "name2", "value");
        Recording.Pause("identifier");
        Recording.Add("identifier", "name3", "value");
        return Verify(Recording.Stop("identifier"));
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
        Recording.Start("identifier");
        Recording.Add("identifier", "name1", "value1");
        Recording.Add("identifier", "name2", "value2");
        return Verify(Recording.Stop("identifier"));
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
        Recording.Start("identifier");
        Recording.Add("identifier", "name", "value1");
        Recording.Add("identifier", "name", "value2");
        return Verify(Recording.Stop("identifier"));
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
        Recording.Start("identifier");
        Recording.Add("identifier", "name", "value1");
        Recording.Add("identifier", "Name", "value2");
        return Verify(Recording.Stop("identifier"));
    }
}