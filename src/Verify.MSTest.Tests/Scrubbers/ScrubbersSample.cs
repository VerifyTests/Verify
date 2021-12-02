namespace TheTests;

#region ScrubbersSampleMSTest

[TestClass]
public class ScrubbersSample :
    VerifyBase
{
    [TestMethod]
    public Task Lines()
    {
        var settings = new VerifySettings();
        settings.ScrubLinesWithReplace(
            replaceLine: line =>
            {
                if (line.Contains("LineE"))
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
                    if (line.Contains("LineE"))
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
        var target = new ToBeScrubbed
        {
            RowVersion = "7D3"
        };

        var settings = new VerifySettings();
        settings.AddScrubber(
            input => input.Replace("7D3", "TheRowVersion"));
        return Verify(target, settings);
    }

    [TestMethod]
    public Task AfterSerializationFluent()
    {
        var target = new ToBeScrubbed
        {
            RowVersion = "7D3"
        };

        return Verify(target)
            .AddScrubber(
                input => input.Replace("7D3", "TheRowVersion"));
    }

    [TestMethod]
    public Task RemoveOrReplace()
    {
        return Verify(
                target: @"
                        LineA
                        LineB
                        LineC
                        ")
            .ScrubLinesWithReplace(
                replaceLine: line =>
                {
                    if (line.Contains("LineB"))
                    {
                        return null;
                    }

                    return line.ToLower();
                });
    }

    [TestMethod]
    public Task EmptyLines()
    {
        return Verify(
                target: @"
                        LineA
                        
                        LineC
                        ")
            .ScrubEmptyLines();
    }
}

#endregion