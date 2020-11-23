using System;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

#region ScrubbersSampleXunit

[UsesVerify]
public class ScrubbersSample
{
    [Fact]
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

    [Fact]
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
")
            .ScrubLinesWithReplace(
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

    [Fact]
    public Task AfterSerialization()
    {
        ToBeScrubbed target = new()
        {
            RowVersion = "7D3"
        };

        VerifySettings settings = new();
        settings.AddScrubber(
            input => input.Replace("7D3", "TheRowVersion"));
        return Verifier.Verify(target, settings);
    }

    [Fact]
    public Task AfterSerializationFluent()
    {
        ToBeScrubbed target = new()
        {
            RowVersion = "7D3"
        };

        return Verifier.Verify(target)
            .AddScrubber(
                input => input.Replace("7D3", "TheRowVersion"));
    }
}

#endregion