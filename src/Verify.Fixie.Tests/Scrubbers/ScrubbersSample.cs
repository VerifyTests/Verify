#region ScrubbersSampleFixie

public class ScrubbersSample
{
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

    public Task EmptyLines() =>
        Verify("""

               LineA

               LineC

               """)
            .ScrubEmptyLines();
}

#endregion