public class RawTempUsage
{
    [Fact]
    public void Directory()
    {
        using var directory = new TempDirectory();
    }

    [Fact]
    public void File()
    {
        using var file = new TempFile();
    }
}