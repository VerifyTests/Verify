// Lives here, rather than in Verify.Tests, since IgnoreTrailingNewline is a static setting, and
// this project runs serially with BaseTest resetting it between tests.
public class IgnoreTrailingNewlineTests :
    BaseTest
{
    [Fact]
    public async Task NotIgnoredByDefault()
    {
        using var temp = new TempDirectory();
        await File.WriteAllTextAsync(VerifiedPath(temp), "a\n");

        await Assert.ThrowsAsync<VerifyException>(() => Verify("a", Settings(temp)));
    }

    [Fact]
    public async Task Ignored()
    {
        VerifierSettings.IgnoreTrailingNewline();

        using var temp = new TempDirectory();
        await File.WriteAllTextAsync(VerifiedPath(temp), "a\n");

        await Verify("a", Settings(temp));
    }

    [Fact]
    public async Task IgnoredWhenReceivedAlsoEndsInNewline()
    {
        VerifierSettings.IgnoreTrailingNewline();

        using var temp = new TempDirectory();
        await File.WriteAllTextAsync(VerifiedPath(temp), "a\n\n");

        await Verify("a\n", Settings(temp));
    }

    [Fact]
    public async Task OnlyASingleNewline()
    {
        VerifierSettings.IgnoreTrailingNewline();

        using var temp = new TempDirectory();
        await File.WriteAllTextAsync(VerifiedPath(temp), "a\n\n");

        await Assert.ThrowsAsync<VerifyException>(() => Verify("a", Settings(temp)));
    }

    [Fact]
    public async Task OnlyWhereTheNewlineIsTheSoleDifference()
    {
        VerifierSettings.IgnoreTrailingNewline();

        using var temp = new TempDirectory();
        await File.WriteAllTextAsync(VerifiedPath(temp), "b\n");

        await Assert.ThrowsAsync<VerifyException>(() => Verify("a", Settings(temp)));
    }

    [Fact]
    public async Task OnlyForVerified()
    {
        VerifierSettings.IgnoreTrailingNewline();

        using var temp = new TempDirectory();
        await File.WriteAllTextAsync(VerifiedPath(temp), "a");

        // The tolerance exists for editors adding a final newline to verified. Received is
        // written by Verify, so a trailing newline there is part of what the test produced.
        await Assert.ThrowsAsync<VerifyException>(() => Verify("a\n", Settings(temp)));
    }

    [Fact]
    public async Task VerifiedIsNotRewritten()
    {
        VerifierSettings.IgnoreTrailingNewline();

        using var temp = new TempDirectory();
        var verified = VerifiedPath(temp);
        await File.WriteAllTextAsync(verified, "a\n");

        await Verify("a", Settings(temp));

        // Trimming happens in memory. A passing test does not write, so the trailing newline
        // stays on disk.
        Assert.Equal("a\n", await File.ReadAllTextAsync(verified));
    }

    static VerifySettings Settings(TempDirectory temp)
    {
        var settings = new VerifySettings();
        settings.UseDirectory(temp);
        // Several verifies above are expected to fail, so without this the diff tool is launched
        settings.DisableDiff();
        return settings;
    }

    static string VerifiedPath(TempDirectory temp, [CallerMemberName] string name = "") =>
        temp.BuildPath($"{nameof(IgnoreTrailingNewlineTests)}.{name}.verified.txt");
}
