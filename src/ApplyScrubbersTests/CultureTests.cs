// Inline date scrubbers registered without an explicit culture must follow the
// culture in effect when the scrub runs, not the one in effect at registration.
public class CultureTests
{
    static readonly CultureInfo enUs = new("en-US");
    static readonly CultureInfo deDe = new("de-DE");

    static string RunInCulture(CultureInfo culture, string input, params Scrubber[] scrubbers)
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = culture;
            return EngineRunner.Run(input, scrubbers);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    [Fact]
    public void ScrubTimeCultureIsUsedWhenNoneSupplied()
    {
        // Registered under en-US, as a module initializer would be
        var original = CultureInfo.CurrentCulture;
        Scrubber[] scrubbers;
        try
        {
            CultureInfo.CurrentCulture = enUs;
            scrubbers = DateMatchers.DateTimes("dd MMMM yyyy", null);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }

        var german = new DateTime(2024, 12, 5).ToString("dd MMMM yyyy", deDe);
        var american = new DateTime(2024, 12, 5).ToString("dd MMMM yyyy", enUs);

        Assert.Equal("[DateTime_1]", RunInCulture(deDe, $"[{german}]", scrubbers));
        Assert.Equal("[DateTime_1]", RunInCulture(enUs, $"[{american}]", scrubbers));
    }

    [Fact]
    public void ExplicitCultureIsNotAffectedByScrubTimeCulture()
    {
        var scrubbers = DateMatchers.DateTimes("dd MMMM yyyy", enUs);

        var american = new DateTime(2024, 12, 5).ToString("dd MMMM yyyy", enUs);
        var german = new DateTime(2024, 12, 5).ToString("dd MMMM yyyy", deDe);

        // The explicit culture wins even while another culture is current
        Assert.Equal("[DateTime_1]", RunInCulture(deDe, $"[{american}]", scrubbers));
        Assert.Equal($"[{german}]", RunInCulture(deDe, $"[{german}]", scrubbers));
    }

    [Fact]
    public void CultureBoundsFollowScrubTimeCulture()
    {
        // Month name lengths differ between cultures, so the window bounds have to
        // be rebuilt per culture and not just the parse
        var original = CultureInfo.CurrentCulture;
        Scrubber[] scrubbers;
        try
        {
            CultureInfo.CurrentCulture = enUs;
            scrubbers = DateMatchers.DateTimes("MMMM d yyyy", null);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }

        var french = new CultureInfo("fr-FR");
        var rendered = new DateTime(2024, 2, 1).ToString("MMMM d yyyy", french);
        Assert.Equal("[DateTime_1]", RunInCulture(french, $"[{rendered}]", scrubbers));
    }

    [Fact]
    public void RepeatedScrubsReuseTheCachedCultureScrubber()
    {
        var original = CultureInfo.CurrentCulture;
        Scrubber[] scrubbers;
        try
        {
            CultureInfo.CurrentCulture = enUs;
            scrubbers = DateMatchers.DateTimes("dd MMMM yyyy", null);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }

        var german = new DateTime(2024, 12, 5).ToString("dd MMMM yyyy", deDe);
        for (var index = 0; index < 3; index++)
        {
            Assert.Equal("[DateTime_1]", RunInCulture(deDe, $"[{german}]", scrubbers));
        }
    }
}
