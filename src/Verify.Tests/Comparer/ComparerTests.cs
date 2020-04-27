using System;
using System.IO;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ComparerTests :
    VerifyBase
{
    [Fact]
    public async Task Instance()
    {
        var settings = new VerifySettings();
        settings.UseComparer(Compare);
        settings.UseExtension("instanceComparerExt");
        await Verify("TheText", settings);
        await Verify("thetext", settings);
    }

    [Fact]
    public async Task Static()
    {
        SharedVerifySettings.RegisterComparer("staticComparerExt", Compare);
        var settings = new VerifySettings();
        settings.UseExtension("staticComparerExt");
        await Verify("TheText", settings);
        await Verify("thetext", settings);
    }

    static async Task<bool> Compare(VerifySettings settings, Stream received, Stream verified)
    {
        var stringOne = await received.ReadString();
        var stringTwo = await verified.ReadString();
        return string.Equals(stringOne, stringTwo, StringComparison.OrdinalIgnoreCase);
    }

    public ComparerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}