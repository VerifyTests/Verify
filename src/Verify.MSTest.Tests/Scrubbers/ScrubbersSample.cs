using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyTests;
using VerifyMSTest;

#region ScrubbersSampleMSTest

[TestClass]
public class ScrubbersSample :
    VerifyBase
{
    [TestMethod]
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

    [TestMethod]
    public Task LinesFluent()
    {
        return Verify(
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

    [TestMethod]
    public Task AfterSerialization()
    {
        ToBeScrubbed target = new()
        {
            RowVersion = "7D3"
        };

        VerifySettings settings = new();
        settings.AddScrubber(
            input => input.Replace("7D3", "TheRowVersion"));
        return Verify(target, settings);
    }

    [TestMethod]
    public Task AfterSerializationFluent()
    {
        ToBeScrubbed target = new()
        {
            RowVersion = "7D3"
        };

        return Verify(target)
            .AddScrubber(
                input => input.Replace("7D3", "TheRowVersion"));
    }
}

#endregion