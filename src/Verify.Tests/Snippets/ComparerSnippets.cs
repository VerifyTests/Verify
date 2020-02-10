using System;
using System.IO;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit.Abstractions;

public class ComparerSnippets :
    VerifyBase
{
    public ComparerSnippets(ITestOutputHelper output) :
        base(output)
    {
    }

    public async Task InstanceComparer()
    {
        #region InstanceComparer
        var settings = new VerifySettings();
        settings.UseComparer(Compare);
        settings.UseExtension("instanceComparerExt");
        await Verify("TheText", settings);
        #endregion
    }

    public async Task StaticComparer()
    {
        #region StaticComparer
        SharedVerifySettings.RegisterComparer("caseless", Compare);
        var settings = new VerifySettings();
        settings.UseExtension("caseless");
        await Verify("TheText", settings);
        #endregion
    }

    #region IgnoreCaseCompare
    static bool Compare(Stream one, Stream two)
    {
        using var reader1 = new StreamReader(one);
        var stringOne = reader1.ReadToEnd();
        using var reader2 = new StreamReader(two);
        var stringTwo = reader2.ReadToEnd();
        return string.Equals(stringOne, stringTwo, StringComparison.OrdinalIgnoreCase);
    }
    #endregion
}