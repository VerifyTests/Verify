#region ScrubbersSampleTUnit

public class ScrubbersSample
{
    [Test]
    public Task Lines()
    {
        var settings = new VerifySettings();
        settings.ScrubLinesWithReplace(
            replaceLine: (ReadOnlySpan<char> line, out ReadOnlySpan<char> replacement) =>
            {
                if (line.Contains("LineE", StringComparison.Ordinal))
                {
                    replacement = "NoMoreLineE".AsSpan();
                    return true;
                }

                replacement = line;
                return true;
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

    [Test]
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
                replaceLine: (ReadOnlySpan<char> line, out ReadOnlySpan<char> replacement) =>
                {
                    if (line.Contains("LineE", StringComparison.Ordinal))
                    {
                        replacement = "NoMoreLineE".AsSpan();
                        return true;
                    }

                    replacement = line;
                    return true;
                })
            .ScrubLines(removeLine: (ReadOnlySpan<char> line) => line.IndexOf('J') != -1)
            .ScrubLinesContaining("b", "D")
            .ScrubLinesContaining(StringComparison.Ordinal, "H");

    [Test]
    public Task RemoveOrReplace() =>
        Verify("""
               LineA
               LineB
               LineC
               """)
            .ScrubLinesWithReplace(
                replaceLine: (ReadOnlySpan<char> line, out ReadOnlySpan<char> replacement) =>
                {
                    if (line.Contains("LineB", StringComparison.Ordinal))
                    {
                        replacement = default;
                        return false;
                    }

                    replacement = line.ToString().ToLower().AsSpan();
                    return true;
                });

    [Test]
    public Task EmptyLines() =>
        Verify("""

               LineA

               LineC

               """)
            .ScrubEmptyLines();
}

#endregion