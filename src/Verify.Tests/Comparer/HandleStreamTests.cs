// A FileStream built from a handle may have no usable Name: .NET Framework reports
// "[Unknown]", and modern .NET falls back to that when the path cannot be resolved
// from the handle. The New path already copes, so only a mismatch exercised it.
public class HandleStreamTests
{
    [Fact]
    public async Task Mismatch()
    {
        using var directory = new TempDirectory();
        var path = directory.BuildPath("source.txt");
        File.WriteAllText(path, "TheReceivedValue");

        using var source = File.OpenRead(path);
        using var stream = new FileStream(source.SafeFileHandle, FileAccess.Read);

        var settings = new VerifySettings();
        settings.DisableDiff();

        await Assert.ThrowsAsync<VerifyException>(() => Verify(stream, "bin", settings));

        var received = CurrentFile.Relative($"HandleStreamTests.Mismatch.{Namer.RuntimeAndVersion}.received.bin");
        Assert.Equal("TheReceivedValue", File.ReadAllText(received));
    }
}
