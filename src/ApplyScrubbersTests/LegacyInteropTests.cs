public class LegacyInteropTests
{
    static LegacyInteropTests() =>
        EngineRunner.UseFakeDirectories();

    static string RunForExtension(string input, Action<VerifySettings> configure)
    {
        var settings = new VerifySettings();
        configure(settings);
        using var counter = Counter.Start();
        var builder = new StringBuilder(input);
        ApplyScrubbers.ApplyForExtension("txt", builder, settings, counter);
        return builder.ToString();
    }

    [Fact]
    public void LegacyRunsAfterEngine()
    {
        // The span scrubber quarantines its replacement from other span scrubbers,
        // but the legacy pass still sees and may modify it
        var result = RunForExtension(
            "a",
            settings =>
            {
                settings.AddScrubber(Scrubber.Replace("a", "1"));
                settings.AddScrubber(builder => builder.Replace("1", "2"));
            });
        Assert.Equal("2", result);
    }

    [Fact]
    public void LegacyLocations_StillHonored()
    {
        var result = RunForExtension(
            "value",
            settings =>
            {
                settings.AddScrubber(builder => builder.Append(" one"), ScrubberLocation.Last);
                settings.AddScrubber(builder => builder.Append(" two"), ScrubberLocation.Last);
            });
        Assert.Equal("value one two", result);
    }

    [Fact]
    public void FixNewlines_AfterLegacy()
    {
        var result = RunForExtension(
            "value",
            settings => settings.AddScrubber(builder => builder.Append("\r\nline")));
        Assert.Equal("value\nline", result);
    }

    [Fact]
    public void PathReplacements_AfterLegacy()
    {
        // A legacy scrubber writes a raw path; the trailing path replacement pass
        // still converts it
        var result = RunForExtension(
            "value ",
            settings => settings.AddScrubber(builder => builder.Append("C:/Code/TheSolution/TheProject/file.txt")));
        Assert.Equal("value {ProjectDirectory}file.txt", result);
    }

    [Fact]
    public void LegacyUserScrubber_BeatsPathReplacements()
    {
        // MoreSpecificScrubberShouldOverride semantics: the legacy scrubber sees the
        // raw path before the path replacement pass
        var result = RunForExtension(
            "C:/Code/TheSolution/TheProject/data",
            settings => settings.AddScrubber(builder => builder.Replace("C:/Code/TheSolution/TheProject/data", "{custom}")));
        Assert.Equal("{custom}", result);
    }

    [Fact]
    public void NoLegacy_EngineOnlyPath()
    {
        var result = RunForExtension(
            "C:/Code/TheSolution/TheProject/file.txt abc",
            settings => settings.AddScrubber(Scrubber.Replace("abc", "xyz")));
        Assert.Equal("{ProjectDirectory}file.txt xyz", result);
    }

    [Fact]
    public void ExtensionMapped_SpanScrubberApplies()
    {
        var result = RunForExtension(
            "abc",
            settings => settings.AddScrubber("txt", Scrubber.Replace("abc", "xyz")));
        Assert.Equal("xyz", result);
    }

    [Fact]
    public void ExtensionMapped_OtherExtensionIgnored()
    {
        var result = RunForExtension(
            "abc",
            settings => settings.AddScrubber("json", Scrubber.Replace("abc", "xyz")));
        Assert.Equal("abc", result);
    }

    [Fact]
    public void DisableScrubbers_BypassesEverything()
    {
        var result = RunForExtension(
            "abc\r\n",
            settings =>
            {
                settings.AddScrubber(Scrubber.Replace("abc", "xyz"));
                settings.DisableScrubbers();
            });
        Assert.Equal("abc\n", result);
    }

    [Fact]
    public void SettingsClone_CopiesSpanScrubbers()
    {
        var settings = new VerifySettings();
        settings.AddScrubber(Scrubber.Replace("abc", "xyz"));
        settings.AddScrubber("txt", Scrubber.Replace("def", "uvw"));

        var clone = new VerifySettings(settings);
        using var counter = Counter.Start();
        var builder = new StringBuilder("abc def");
        ApplyScrubbers.ApplyForExtension("txt", builder, clone, counter);
        Assert.Equal("xyz uvw", builder.ToString());

        // Mutating the clone must not affect the original
        clone.AddScrubber(Scrubber.Replace("ghi", "rst"));
        using var secondCounter = Counter.Start();
        var second = new StringBuilder("ghi");
        ApplyScrubbers.ApplyForExtension("txt", second, settings, secondCounter);
        Assert.Equal("ghi", second.ToString());
    }
}
