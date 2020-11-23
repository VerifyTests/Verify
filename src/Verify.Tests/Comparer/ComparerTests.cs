using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;
using Xunit.Sdk;

[UsesVerify]
public class ComparerTests
{
    [Fact]
    public async Task Instance_with_message()
    {
        VerifySettings settings = new();
        settings.UseComparer(CompareWithMessage);
        settings.DisableDiff();
        settings.DisableClipboard();
        var exception = await Assert.ThrowsAsync<XunitException>(() => Verifier.Verify("NotTheText", settings));
        Assert.Contains("theMessage", exception.Message);
    }

    [Fact]
    public async Task Instance()
    {
        VerifySettings settings = new();
        settings.UseComparer(Compare);
        await Verifier.Verify("TheText", settings);
        await Verifier.Verify("thetext", settings);
    }

    [Fact]
    public async Task Static_with_message()
    {
        VerifierSettings.RegisterComparer("staticComparerExtMessage", CompareWithMessage);
        VerifySettings settings = new();
        settings.UseExtension("staticComparerExtMessage");
        settings.DisableDiff();
        settings.DisableClipboard();
        var exception = await Assert.ThrowsAsync<XunitException>(() => Verifier.Verify("TheText", settings));
        Assert.Contains("theMessage", exception.Message);
    }

    [Fact]
    public async Task Static()
    {
        VerifierSettings.RegisterComparer("staticComparerExt", Compare);
        VerifySettings settings = new();
        settings.UseExtension("staticComparerExt");
        await Verifier.Verify("TheText", settings);
        await Verifier.Verify("thetext", settings);
    }

    static async Task<CompareResult> Compare(Stream received, Stream verified, IReadOnlyDictionary<string, object> context)
    {
        var stringOne = await received.ReadString();
        var stringTwo = await verified.ReadString();
        return new(string.Equals(stringOne, stringTwo, StringComparison.OrdinalIgnoreCase));
    }

    static Task<CompareResult> CompareWithMessage(Stream stream, Stream received, IReadOnlyDictionary<string, object> readOnlyDictionary)
    {
        return Task.FromResult(CompareResult.NotEqual("theMessage"));
    }
}