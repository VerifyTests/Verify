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

#if NET6_0

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
        EmptyFiles.Extensions.AddTextExtension("staticComparerExtMessage");
        VerifierSettings.RegisterStringComparer("staticComparerExtMessage", CompareWithMessage);
        var settings = new VerifySettings();
        settings.UseExtension("staticComparerExtMessage");
        settings.DisableDiff();
        settings.UseMethodName("Static_with_message_temp");
        await ThrowsTask(() => Verify("TheText", settings));
    }

    static Task<CompareResult> CompareWithMessage(string stream, string received, IReadOnlyDictionary<string, object> readOnlyDictionary) =>
        Task.FromResult(CompareResult.NotEqual("theMessage"));

#endif

    [Fact]
    public async Task Static()
    {
        EmptyFiles.Extensions.AddTextExtension("staticComparerExt");
        VerifierSettings.RegisterStringComparer("staticComparerExt", Compare);
        var settings = new VerifySettings();
        settings.UseExtension("staticComparerExt");
        await Verify("TheText", settings);
        PrefixUnique.Clear();
        await Verify("thetext", settings);
    }

    static Task<CompareResult> Compare(string received, string verified, IReadOnlyDictionary<string, object> context) =>
        Task.FromResult(new CompareResult(string.Equals(received, received, StringComparison.OrdinalIgnoreCase)));
}