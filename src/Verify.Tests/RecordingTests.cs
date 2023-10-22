[UsesVerify]
public class RecordingTests
{
    [Fact]
    public Task Simple()
    {
        Recording.Add("name", "value");
        return Verify("TheValue");
    }

    [Fact]
    public Task NoValue()
    {
        Recording.Add("name", "value");
        return Verify();
    }

    [Fact]
    public Task Multiple()
    {
        Recording.Add("name1", "value1");
        Recording.Add("name2", "value2");
        return Verify("TheValue");
    }

    [Fact]
    public Task Append()
    {
        Recording.Add("name", "value1");
        Recording.Add("name", "value2");
        return Verify("TheValue");
    }

    [Fact]
    public Task Case()
    {
        Recording.Add("name", "value1");
        Recording.Add("Name", "value2");
        return Verify("TheValue");
    }
}