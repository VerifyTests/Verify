
[UsesVerify]
public class RecordingTests
{
    [Fact]
    public Task Simple()
    {
        Recording.Add("name","value");
        return Verify("TheValue");
    }
}