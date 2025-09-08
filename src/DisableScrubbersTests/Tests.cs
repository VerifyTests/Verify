public class Tests
{
    static Tests()
    {
        VerifierSettings.AssignTargetAssembly(typeof(Tests).Assembly);
        ApplyScrubbers.UseAssembly("TheSolutionDir", "TheProjectDir");
    }

    [Fact]
    public Task Test() =>
        Verify(BuildTarget())
            .DisableScrubbers();

    [Fact]
    public Task TestWithExtension()
    {
        var json = JsonConvert.SerializeObject(BuildTarget(), Formatting.Indented);
        return Verify(json, extension: "json")
            .ScrubInlineGuids()
            .ScrubInlineDates("yyyy-MM-dd")
            .ScrubInlineDateTimes("yyyy-MM-ddTHH:mm:ss")
            .ScrubInlineDateTimeOffsets("yyyy-MM-ddTHH:mm:sszzz")
            .DisableScrubbers();
    }

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
}