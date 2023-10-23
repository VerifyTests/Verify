[UsesVerify]
public class RecordingTests
{
    [Fact]
    public Task Simple()
    {
        Recording.Start();
        Recording.Add("name", "value");
        return Verify("TheValue");
    }

    [Fact]
    public Task Clear()
    {
        Recording.Start();
        Recording.Add("name1", "value");
        Recording.Clear();
        Recording.Add("name2", "value");
        return Verify("TheValue");
    }

    [Fact]
    public Task PauseResume()
    {
        Recording.Start();
        Recording.Pause();
        Recording.Add("name1", "value");
        Recording.Resume();
        Recording.Add("name2", "value");
        Recording.Pause();
        Recording.Add("name3", "value");
        return Verify("TheValue");
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
    public Task Append()
    {
        Recording.Start();
        Recording.Add("name", "value1");
        Recording.Add("name", "value2");
        return Verify("TheValue");
    }

    [Fact]
    public Task Case()
    {
        Recording.Start();
        Recording.Add("name", "value1");
        Recording.Add("Name", "value2");
        return Verify("TheValue");
    }
}