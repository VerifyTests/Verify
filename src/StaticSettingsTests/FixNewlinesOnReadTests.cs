// Lives here, rather than in Verify.Tests, since FixNewlinesOnRead is a static setting, and this
// project runs serially with BaseTest resetting it between tests.
public class FixNewlinesOnReadTests :
    BaseTest
{
    [Fact]
    public async Task RejectedByDefault()
    {
        using var temp = new TempDirectory();
        await File.WriteAllTextAsync(VerifiedPath(temp), "a\r\nb");

        var exception = await Assert.ThrowsAnyAsync<Exception>(() => Verify("a\nb", Settings(temp)));

        Assert.Contains("carriage return", exception.Message);
    }

    [Fact]
    public async Task Crlf()
    {
        VerifierSettings.FixNewlinesOnRead();

        using var temp = new TempDirectory();
        await File.WriteAllTextAsync(VerifiedPath(temp), "a\r\nb");

        await Verify("a\nb", Settings(temp));
    }

    [Fact]
    public async Task Cr()
    {
        VerifierSettings.FixNewlinesOnRead();

        using var temp = new TempDirectory();
        await File.WriteAllTextAsync(VerifiedPath(temp), "a\rb");

        await Verify("a\nb", Settings(temp));
    }

    [Fact]
    public async Task VerifiedIsNotRewritten()
    {
        VerifierSettings.FixNewlinesOnRead();

        using var temp = new TempDirectory();
        var verified = VerifiedPath(temp);
        await File.WriteAllTextAsync(verified, "a\r\nb");

        await Verify("a\nb", Settings(temp));

        // Normalizing happens in memory. A passing test does not write, so the file on disk
        // keeps its line endings.
        Assert.Equal("a\r\nb", await File.ReadAllTextAsync(verified));
    }

    [Fact]
    public async Task MismatchStillFails()
    {
        VerifierSettings.FixNewlinesOnRead();

        using var temp = new TempDirectory();
        await File.WriteAllTextAsync(VerifiedPath(temp), "a\r\nb");

        // Line endings are normalized, the rest of the content is still compared as is
        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify("a\nc", Settings(temp)));

        Assert.DoesNotContain("carriage return", exception.Message);
        Assert.Equal("a\nc", await File.ReadAllTextAsync(ReceivedPath(temp)));
    }

    static VerifySettings Settings(TempDirectory temp)
    {
        var settings = new VerifySettings();
        settings.UseDirectory(temp);
        // Several verifies below are expected to fail, so without this the diff tool is launched
        settings.DisableDiff();
        return settings;
    }

    static string VerifiedPath(TempDirectory temp, [CallerMemberName] string name = "") =>
        temp.BuildPath($"{nameof(FixNewlinesOnReadTests)}.{name}.verified.txt");

    static string ReceivedPath(TempDirectory temp, [CallerMemberName] string name = "") =>
        temp.BuildPath($"{nameof(FixNewlinesOnReadTests)}.{name}.received.txt");
}
