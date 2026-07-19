public class FastPathTests
{
    static FastPathTests() =>
        EngineRunner.UseFakeDirectories();

    static string Apply(string value, Action<VerifySettings>? configure = null)
    {
        var settings = new VerifySettings();
        configure?.Invoke(settings);
        using var counter = Counter.Start();
        return ApplyScrubbers.ApplyForPropertyValue(value, settings, counter);
    }

    [Fact]
    public void ShortValue_NoScrubbers_SameInstance()
    {
        var value = "short";
        Assert.Same(value, Apply(value));
    }

    [Fact]
    public void CarriageReturn_ForcesNormalization()
    {
        var value = "a\r\nb";
        var result = Apply(value);
        Assert.Equal("a\nb", result);
    }

    [Fact]
    public void ValueShorterThanScrubberMin_SameInstance()
    {
        var value = "ab";
        var result = Apply(value, settings => settings.AddScrubber(Scrubber.Replace("abcdef", "x")));
        Assert.Same(value, result);
    }

    [Fact]
    public void MatchingValue_Scrubbed()
    {
        var result = Apply("abcdef!", settings => settings.AddScrubber(Scrubber.Replace("abcdef", "x")));
        Assert.Equal("x!", result);
    }

    [Fact]
    public void UnknownMinScrubber_DisablesFastPath()
    {
        var invoked = false;
        Apply(
            "a",
            settings => settings.AddScrubber(Scrubber.Match(
                (CharSpan _, Counter _, IReadOnlyDictionary<string, object> _, out int index, out int length, out string? replacement) =>
                {
                    invoked = true;
                    index = 0;
                    length = 0;
                    replacement = null;
                    return false;
                })));
        Assert.True(invoked);
    }

    [Fact]
    public void PathValue_Replaced()
    {
        var result = Apply("C:/Code/TheSolution/TheProject/file.txt");
        Assert.Equal("{ProjectDirectory}file.txt", result);
    }

    [Fact]
    public void LegacyScrubber_ForcesFullPath()
    {
        var result = Apply(
            "ab",
            settings => settings.AddScrubber(builder => builder.Append('!')));
        Assert.Equal("ab!", result);
    }

    [Fact]
    public void ExtensionMappedScrubbers_ExcludedFromPropertyPath()
    {
        var value = "abc";
        var result = Apply(value, settings => settings.AddScrubber("txt", Scrubber.Replace("abc", "xyz")));
        Assert.Same(value, result);
    }

    [Fact]
    public void ScrubbersDisabled_OnlyNormalizes()
    {
        var result = Apply(
            "abc\r\n",
            settings =>
            {
                settings.AddScrubber(Scrubber.Replace("abc", "xyz"));
                settings.DisableScrubbers();
            });
        Assert.Equal("abc\n", result);
    }

    [Fact]
    public void NoMatch_EngineStillZeroCopy()
    {
        // Long enough to enter the engine, but nothing matches: same instance back
        var value = new string('x', 200);
        var result = Apply(value, settings => settings.AddScrubber(Scrubber.Replace("missing", "y")));
        Assert.Same(value, result);
    }
}
