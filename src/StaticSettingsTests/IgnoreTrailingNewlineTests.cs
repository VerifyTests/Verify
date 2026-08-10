// Lives here, rather than in Verify.Tests, since IgnoreTrailingNewline is a static setting, and
// this project runs serially with BaseTest resetting it between tests.
[SuppressMessage("Performance", "CA1857:A constant is expected for the parameter")]
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

    // The inline path implements the same tolerance against the literal rather than a
    // verified file, and the literal's line endings come from the .cs file it lives in

    [Fact]
    public async Task InlineNotIgnoredByDefault()
    {
        using var temp = new TempDirectory();

        await Assert.ThrowsAsync<VerifyException>(() => Verify("a", InlineSettings(temp, "a\n")));
    }

    [Fact]
    public async Task InlineIgnored()
    {
        VerifierSettings.IgnoreTrailingNewline();

        using var temp = new TempDirectory();

        await Verify("a", InlineSettings(temp, "a\n"));
    }

    [Fact]
    public async Task InlineIgnoredForCrlfLiteral()
    {
        VerifierSettings.IgnoreTrailingNewline();

        using var temp = new TempDirectory();

        // Normalized to "a\n" first, then the trailing newline is trimmed
        await Verify("a", InlineSettings(temp, "a\r\n"));
    }

    [Fact]
    public async Task InlineOnlyASingleNewline()
    {
        VerifierSettings.IgnoreTrailingNewline();

        using var temp = new TempDirectory();

        await Assert.ThrowsAsync<VerifyException>(() => Verify("a", InlineSettings(temp, "a\n\n")));
    }

    [Fact]
    public async Task InlineOnlyWhereTheNewlineIsTheSoleDifference()
    {
        VerifierSettings.IgnoreTrailingNewline();

        using var temp = new TempDirectory();

        await Assert.ThrowsAsync<VerifyException>(() => Verify("a", InlineSettings(temp, "b\n")));
    }

    static VerifySettings InlineSettings(TempDirectory temp, string expected)
    {
        var settings = Settings(temp);
        // Deliberately not this file: the failing cases stage a patch, and pointing it at
        // real source would let a tray accept rewrite it
        settings.Snapshot(expected, temp.BuildPath("Fake.cs"), 1, "\"ignored\"");
        return settings;
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
