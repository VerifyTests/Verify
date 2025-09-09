public class Tests
{
    static Tests()
    {
        VerifierSettings.AssignTargetAssembly(typeof(Tests).Assembly);
        ApplyScrubbers.UseAssembly("TheSolutionDir", "TheProjectDir");
    }

    #region DisableScrubbers

    [Fact]
    public Task Instance()
    {
        var settings = new VerifySettings();
        settings.DisableScrubbers();
        return Verify(BuildTarget(), settings);
    }

    #endregion

    #region DisableScrubbersFluent

    [Fact]
    public Task Fluent() =>
        Verify(BuildTarget())
            .DisableScrubbers();

    #endregion

    [Fact]
    public Task ClonedSettings()
    {
        var settings = new VerifySettings();
        settings.DisableScrubbers();
        var cloned = new VerifySettings(settings);
        return Verify(BuildTarget(), cloned);
    }

    [Fact]
    public Task WithExtension()
    {
        var settings = new VerifySettings();
        settings.ScrubInlineGuids();
        settings.ScrubInlineDates("yyyy-MM-dd");
        settings.ScrubInlineDateTimes("yyyy-MM-ddTHH:mm:ss");
        settings.ScrubInlineDateTimeOffsets("yyyy-MM-ddTHH:mm:sszzz");
        settings.DisableScrubbers();
        var json = JsonConvert.SerializeObject(BuildTarget(), Formatting.Indented);
        return Verify(json, extension: "json");
    }

    [Fact]
    public Task WithExtensionFluent()
    {
        var json = JsonConvert.SerializeObject(BuildTarget(), Formatting.Indented);
        return Verify(json, extension: "json")
            .ScrubInlineGuids()
            .ScrubInlineDates("yyyy-MM-dd")
            .ScrubInlineDateTimes("yyyy-MM-ddTHH:mm:ss")
            .ScrubInlineDateTimeOffsets("yyyy-MM-ddTHH:mm:sszzz")
            .DisableScrubbers();
    }

    #region DisableScrubbersTarget

    static object BuildTarget() =>
        new
        {
            TheSolutionDir = "TheSolutionDir",
            TheProjectDir = "TheProjectDir",
            Date = new Date(2020, 1, 1),
            DateTime = new DateTime(2020, 1, 1),
            DateTimeOffset = new DateTimeOffset(2020, 1, 1, 1, 1, 1, TimeSpan.FromHours(10)),
            Guid = new Guid("ae8529a6-30a0-46e2-b7d6-9fcb7b23463c"),
        };

    #endregion
}