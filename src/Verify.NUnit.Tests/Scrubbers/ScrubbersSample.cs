using System;
using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;
using VerifyTests;

#region ScrubbersSampleNUnit

[TestFixture]
public class ScrubbersSample
{
    [Test]
    public Task Lines()
    {
        VerifySettings settings = new();
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
        return Verifier.Verify(
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
    public Task LinesFluent()
    {
        return Verifier.Verify(
                target: @"
LineA
LineB
LineC
LineD
LineE
LineH
LineI
LineJ
").ScrubLinesWithReplace(
                replaceLine: line =>
                {
                    if (line == "LineE")
                    {
                        return "NoMoreLineE";
                    }

                    return line;
                })
            .ScrubLines(removeLine: line => line.Contains("J"))
            .ScrubLinesContaining("b", "D")
            .ScrubLinesContaining(StringComparison.Ordinal, "H");
    }

    [Test]
    public Task AfterSerialization()
    {
        ToBeScrubbed target = new()
        {
            RowVersion = "7D3"
        };

        VerifySettings settings = new();
        settings.AddScrubber(
            s => s.Replace("7D3", "TheRowVersion"));
        return Verifier.Verify(target, settings);
    }

    [Test]
    public Task AfterSerializationFluent()
    {
        ToBeScrubbed target = new()
        {
            RowVersion = "7D3"
        };

        return Verifier.Verify(target).AddScrubber(
            s => s.Replace("7D3", "TheRowVersion"));
    }
}

#endregion