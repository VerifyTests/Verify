public class Tests
{
    static Tests()
    {
        VerifierSettings.AssignTargetAssembly(typeof(Tests).Assembly);
        ApplyScrubbers.UseAssembly("C:/Code/TheSolution", "C:/Code/TheSolution/TheProject");
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
    public Task WithConverter()
    {
        var settings = new VerifySettings();
        settings.AddExtraSettings(_ => _.Converters.Add(new Converter()));
        settings.DisableScrubbers();
        return Verify(BuildTarget(), settings);
    }

    class Converter :
        WriteOnlyJsonConverter<Target>
    {
        public override void Write(VerifyJsonWriter writer, Target target)
        {
            var counter = writer.Counter;
            writer.WriteStartObject();
            writer.WriteMember(target, target.TheSolutionDir, "TheSolutionDir");
            writer.WriteMember(target, target.TheProjectDir, "TheProjectDir");
            writer.WriteMember(target, target.Date, "Date");
            writer.WriteMember(target, counter.Next(target.Date), "DateViaCounter");
            writer.WriteMember(target, target.DateTime, "DateTime");
            writer.WriteMember(target, counter.Next(target.DateTime), "DateTimeViaCounter");
            writer.WriteMember(target, target.DateTimeOffset, "DateTimeOffset");
            writer.WriteMember(target, counter.Next(target.DateTimeOffset), "DateTimeOffsetViaCounter");
            writer.WriteMember(target, target.Guid, "Guid");
            writer.WriteMember(target, counter.Next(target.Guid), "GuidViaCounter");

            writer.WritePropertyName("DateTimeMin");
            writer.WriteValue(DateTime.MinValue);
            writer.WritePropertyName("DateTimeMax");
            writer.WriteValue(DateTime.MaxValue);

            writer.WritePropertyName("DateTimeOffsetMin");
            writer.WriteValue(DateTimeOffset.MinValue);
            writer.WritePropertyName("DateTimeOffsetMax");
            writer.WriteValue(DateTimeOffset.MaxValue);

            writer.WritePropertyName("GuidEmpty");
            writer.WriteValue(Guid.Empty);
            writer.WriteEndObject();
        }
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

    public record Target(string TheSolutionDir, string TheProjectDir, Date Date, DateTime DateTime, DateTimeOffset DateTimeOffset, Guid Guid);

    #region DisableScrubbersTarget

    static object BuildTarget() =>
        new Target(
            "C:/Code/TheSolution",
            "C:/Code/TheSolution/TheProject",
            new Date(2020, 1, 1),
            new DateTime(2020, 1, 1),
            new DateTimeOffset(2020, 1, 1, 1, 1, 1, TimeSpan.FromHours(10)),
            new Guid("ae8529a6-30a0-46e2-b7d6-9fcb7b23463c"));

    #endregion
}