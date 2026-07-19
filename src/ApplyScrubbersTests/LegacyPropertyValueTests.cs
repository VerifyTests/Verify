// The property value path when a legacy StringBuilder scrubber is registered:
// span scrubbers run, then the legacy pass, then path replacement and the newline
// fix. Covers ApplyScrubbers.ApplyWithLegacy, which ApplyForExtension does not.
public class LegacyPropertyValueTests
{
    static LegacyPropertyValueTests() =>
        EngineRunner.UseFakeDirectories();

    static string Run(string input, Action<VerifySettings> configure)
    {
        var settings = new VerifySettings();
        configure(settings);
        using var counter = Counter.Start();
        return ApplyScrubbers.ApplyForPropertyValue(input, settings, counter);
    }

    [Fact]
    public void LegacyRunsAfterSpanScrubbers()
    {
        var result = Run(
            "a",
            settings =>
            {
                settings.AddScrubber(Scrubber.Replace("a", "1"));
                settings.AddScrubber(builder => builder.Replace("1", "2"));
            });
        Assert.Equal("2", result);
    }

    [Fact]
    public void PathWrittenByLegacyIsScrubbed()
    {
        var result = Run(
            "value ",
            settings => settings.AddScrubber(builder => builder.Append("C:/Code/TheSolution/TheProject/file.txt")));
        Assert.Equal("value {ProjectDirectory}file.txt", result);
    }

    [Fact]
    public void LegacySeesRawPath()
    {
        var result = Run(
            "C:/Code/TheSolution/TheProject/data",
            settings => settings.AddScrubber(builder => builder.Replace("C:/Code/TheSolution/TheProject/data", "{custom}")));
        Assert.Equal("{custom}", result);
    }

    [Fact]
    public void NewlinesInjectedByLegacyAreNormalized()
    {
        var result = Run(
            "value",
            settings => settings.AddScrubber(builder => builder.Append("\r\nline\rmore")));
        Assert.Equal("value\nline\nmore", result);
    }

    [Fact]
    public void PathAdjacentToInjectedCarriageReturnIsStillScrubbed()
    {
        // Newline normalization now runs before path replacement rather than after,
        // so a path sitting next to an injected '\r' must still be replaced
        var result = Run(
            "value",
            settings => settings.AddScrubber(builder => builder.Append("\r\nC:/Code/TheSolution/TheProject/f.txt\r\n")));
        Assert.Equal("value\n{ProjectDirectory}f.txt\n", result);
    }

    [Fact]
    public void UnchangedValueIsReturnedAsIs()
    {
        var result = Run("plain value", settings => settings.AddScrubber(_ => { }));
        Assert.Equal("plain value", result);
    }
}
