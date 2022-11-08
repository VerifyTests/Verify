[UsesVerify]
public class ComparerTests
{
    [Fact]
    public async Task Instance()
    {
        var settings = new VerifySettings();
        settings.UseStringComparer(Compare);
        await Verify("TheText", settings);
        PrefixUnique.Clear();
        await Verify("thetext", settings);
    }

#if NET7_0_OR_GREATER

    [Fact]
    public async Task Instance_with_message()
    {
        var settings = new VerifySettings();
        settings.UseStringComparer(CompareWithMessage);
        settings.DisableDiff();
        var exception = await Assert.ThrowsAsync<VerifyException>(() => Verify("NotTheText", settings));
        Assert.Contains("theMessage", exception.Message);
    }

    [Fact]
    public async Task Static_with_message()
    {
        FileExtensions.AddTextExtension("staticComparerExtMessage");
        VerifierSettings.RegisterStringComparer("staticComparerExtMessage", CompareWithMessage);
        var settings = new VerifySettings();
        settings.DisableDiff();
        settings.UseMethodName("Static_with_message_temp");
        await ThrowsTask(() => Verify("TheText", "staticComparerExtMessage", settings));
    }

    static Task<CompareResult> CompareWithMessage(string stream, string received, IReadOnlyDictionary<string, object> readOnlyDictionary) =>
        Task.FromResult(CompareResult.NotEqual("theMessage"));

#endif

    [Fact]
    public async Task Static()
    {
        FileExtensions.AddTextExtension("staticComparerExt");
        VerifierSettings.RegisterStringComparer("staticComparerExt", Compare);
        await Verify("TheText", "staticComparerExt");
        PrefixUnique.Clear();
        await Verify("thetext", "staticComparerExt");
    }

    static Task<CompareResult> Compare(string received, string verified, IReadOnlyDictionary<string, object> context) =>
        Task.FromResult(new CompareResult(string.Equals(received, received, StringComparison.OrdinalIgnoreCase)));
}