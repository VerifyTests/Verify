// Lives here, rather than in Verify.Tests, since UseEncoding is a static setting, and this
// project runs serially with BaseTest resetting it between tests.
public class ResultTextEncodingTests :
    BaseTest
{
    // Latin1 writes no preamble, so nothing on disk tells a reader the file is not UTF8. Reading
    // it as UTF8 turns the 0xE9 byte into U+FFFD, which is what makes this the case that catches
    // VerifyResult.Text ignoring the configured encoding.
    const string value = "café";

    [Fact]
    public async Task ReadsExistingVerifiedWithConfiguredEncoding()
    {
        VerifierSettings.UseEncoding(Encoding.Latin1);

        using var temp = new TempDirectory();
        await File.WriteAllTextAsync(VerifiedPath(temp), value, Encoding.Latin1);

        var result = await Verify(value, Settings(temp));

        Assert.Equal(value, result.Text);
    }

    [Fact]
    public async Task RoundTripsWhatVerifyWrote()
    {
        VerifierSettings.UseEncoding(Encoding.Latin1);

        using var temp = new TempDirectory();
        var settings = Settings(temp);
        settings.AutoVerify();

        var result = await Verify(value, settings);

        var file = result.Files.Single();
        Assert.Equal(Encoding.Latin1.GetBytes(value), await File.ReadAllBytesAsync(file));
        Assert.Equal(value, result.Text);
    }

    [Fact]
    public async Task DefaultEncodingIsUnaffected()
    {
        using var temp = new TempDirectory();
        await File.WriteAllTextAsync(VerifiedPath(temp), value, new UTF8Encoding(true));

        var result = await Verify(value, Settings(temp));

        Assert.Equal(value, result.Text);
    }

    static VerifySettings Settings(TempDirectory temp)
    {
        var settings = new VerifySettings();
        settings.UseDirectory(temp);
        settings.DisableDiff();
        return settings;
    }

    static string VerifiedPath(TempDirectory temp, [CallerMemberName] string name = "") =>
        temp.BuildPath($"{nameof(ResultTextEncodingTests)}.{name}.verified.txt");
}
