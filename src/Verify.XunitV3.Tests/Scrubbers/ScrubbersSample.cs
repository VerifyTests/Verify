#region ScrubbersSampleXunit

public class ScrubbersSample
{
    [Fact]
    public Task Lines()
    {
        var settings = new VerifySettings();
        settings.ScrubLinesWithReplace(
            replaceLine: (ReadOnlySpan<char> line) =>
            {
                if (line.Contains("LineE", StringComparison.Ordinal))
                {
                    return "NoMoreLineE";
                }

                return line.ToString();
            });
        settings.ScrubLines(removeLine: (ReadOnlySpan<char> line) => line.IndexOf('J') != -1);
        settings.ScrubLinesContaining("b", "D");
        settings.ScrubLinesContaining(StringComparison.Ordinal, "H");
        return Verify(
            settings: settings,
            target: """
                    LineA
                    LineB
                    LineC
                    LineD
                    LineE
                    LineH
                    LineI
                    LineJ
                    """);
    }

    [Fact]
    public Task EmptyLine()
    {
        var settings = new VerifySettings();
        settings.ScrubLinesWithReplace(
            replaceLine: (ReadOnlySpan<char> _) => "");
        return Verify(
            settings: settings,
            target: "");
    }

    [Fact]
    public Task LinesFluent() =>
        Verify("""
               LineA
               LineB
               LineC
               LineD
               LineE
               LineH
               LineI
               LineJ
               """)
            .ScrubLinesWithReplace(
                replaceLine: (ReadOnlySpan<char> line) =>
                {
                    if (line.Contains("LineE", StringComparison.Ordinal))
                    {
                        return "NoMoreLineE";
                    }

                    return line.ToString();
                })
            .ScrubLines(removeLine: (ReadOnlySpan<char> line) => line.IndexOf('J') != -1)
            .ScrubLinesContaining("b", "D")
            .ScrubLinesContaining(StringComparison.Ordinal, "H");

    [Fact]
    public Task RemoveOrReplace() =>
        Verify("""
               LineA
               LineB
               LineC
               """)
            .ScrubLinesWithReplace(
                replaceLine: (ReadOnlySpan<char> line) =>
                {
                    if (line.Contains("LineB", StringComparison.Ordinal))
                    {
                        return null;
                    }

                    return line.ToString().ToLower();
                });

    [Fact]
    public Task EmptyLines() =>
        Verify("""

               LineA

               LineC

               """)
            .ScrubEmptyLines();
}

#endregion