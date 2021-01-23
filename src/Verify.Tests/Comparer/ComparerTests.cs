using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class ComparerTests
{
    [Fact]
    public async Task Instance_with_message()
    {
        VerifySettings settings = new();
        settings.UseStringComparer(CompareWithMessage);
        settings.DisableDiff();
        settings.DisableClipboard();
        var exception = await Assert.ThrowsAsync<Exception>(() => Verifier.Verify("NotTheText", settings));
        Assert.Contains("theMessage", exception.Message);
    }

    [Fact]
    public async Task Instance()
    {
        VerifySettings settings = new();
        settings.UseStringComparer(Compare);
        await Verifier.Verify("TheText", settings);
        FileNameBuilder.ClearPrefixList();
        await Verifier.Verify("thetext", settings);
    }

    [Fact]
    public async Task Static_with_message()
    {
        VerifierSettings.RegisterStringComparer("staticComparerExtMessage", CompareWithMessage);
        VerifySettings settings = new();
        settings.UseExtension("staticComparerExtMessage");
        settings.DisableDiff();
        settings.DisableClipboard();
        var exception = await Assert.ThrowsAsync<Exception>(() => Verifier.Verify("TheText", settings));
        Assert.Contains("theMessage", exception.Message);
    }

    [Fact]
    public async Task Static()
    {
        VerifierSettings.RegisterStringComparer("staticComparerExt", Compare);
        VerifySettings settings = new();
        settings.UseExtension("staticComparerExt");
        await Verifier.Verify("TheText", settings);
        FileNameBuilder.ClearPrefixList();
        await Verifier.Verify("thetext", settings);
    }

    static Task<CompareResult> Compare(string received, string verified, IReadOnlyDictionary<string, object> context)
    {
        return Task.FromResult(new CompareResult(string.Equals(received, received, StringComparison.OrdinalIgnoreCase)));
    }

    static Task<CompareResult> CompareWithMessage(string stream, string received, IReadOnlyDictionary<string, object> readOnlyDictionary)
    {
        return Task.FromResult(CompareResult.NotEqual("theMessage"));
    }
}