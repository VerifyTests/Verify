using System;
using System.Threading.Tasks;
using NUnit.Framework;
using VerifyTesting;

#region ScrubbersSampleNUnit
using static VerifyNUnit.Verifier;

[TestFixture]
public class ScrubbersSample
{
    [Test]
    public Task Lines()
    {
        var settings = new VerifySettings();
        settings.ScrubLinesWithReplace(
            replaceLine: line =>
            {
                if (line == "LineE")
                {
                    return "NoMoreLineE";
                }

                return line;
            });
        settings.ScrubLines(removeLine: line => line.Contains("J"));
        settings.ScrubLinesContaining("b", "D");
        settings.ScrubLinesContaining(StringComparison.Ordinal, "H");
        return Verify(
            settings: settings,
            target: @"
LineA
LineB
LineC
LineD
LineE
LineH
LineI
LineJ
");
    }

    [Test]
    public Task ScrubberAppliedAfterJsonSerialization()
    {
        var target = new ToBeScrubbed
        {
            RowVersion = "0x00000000000007D3"
        };

        var settings = new VerifySettings();
        settings.AddScrubber(
            s => s.Replace("0x00000000000007D3", "TheRowVersion"));
        return Verify(target, settings);
    }
}
#endregion