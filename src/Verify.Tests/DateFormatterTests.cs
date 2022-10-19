[UsesVerify]
public class DateFormatterTests
{
    [Fact]
    public Task DateTimeOtherTimeZoneToParameterString()
    {
        var dateTime = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Unspecified);
        return Verify(DateFormatter.ToParameterString(new DateTimeOffset(dateTime, TimeSpan.FromHours(1.5))));
    }

    [Fact]
    public Task DateTimeOffsetCustomLocalToParameterString()
    {
        var dateTime = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Local);
        return Verify(DateFormatter.ToParameterString(new DateTimeOffset(dateTime, TimeSpan.FromHours(1.5))));
    }

    [Fact]
    public Task DateTimeLocalToJsonString()
    {
        var dateTime = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Local);
        return Verify(DateFormatter.ToJsonString(dateTime));
    }

    [Fact]
    public Task DateTimeLocalToParameterString()
    {
        var dateTime = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Local);
        return Verify(DateFormatter.ToParameterString(dateTime));
    }

    [Fact]
    public Task DateTimeOffsetLocalToJsonString()
    {
        var dateTime = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Local);
        return Verify(DateFormatter.ToJsonString(new DateTimeOffset(dateTime)));
    }

    [Fact]
    public Task DateTimeOffsetLocalToParameterString()
    {
        var dateTime = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Local);
        return Verify(DateFormatter.ToParameterString(new DateTimeOffset(dateTime)));
    }

    [Fact]
    public Task DateTimeUnspecifiedToJsonString()
    {
        var dateTime = new DateTime(2000, 10, 1, 0, 0, 0);
        return Verify(DateFormatter.ToJsonString(dateTime));
    }

    [Fact]
    public Task DateTimeUnspecifiedToParameterString()
    {
        var dateTime = new DateTime(2000, 10, 1, 0, 0, 0);
        return Verify(DateFormatter.ToParameterString(dateTime));
    }

    [Fact]
    public Task DateTimeOffsetUnspecifiedToJsonString()
    {
        var dateTime = new DateTime(2000, 10, 1, 0, 0, 0);
        return Verify(DateFormatter.ToJsonString(new DateTimeOffset(dateTime)));
    }

    [Fact]
    public Task DateTimeOffsetUnspecifiedToParameterString()
    {
        var dateTime = new DateTime(2000, 10, 1, 0, 0, 0);
        return Verify(DateFormatter.ToParameterString(new DateTimeOffset(dateTime)));
    }

    [Fact]
    public Task DateTimeUtcToJsonString()
    {
        var dateTime = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        return Verify(DateFormatter.ToJsonString(dateTime));
    }

    [Fact]
    public Task DateTimeUtcToParameterString()
    {
        var dateTime = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        return Verify(DateFormatter.ToParameterString(dateTime));
    }

    [Fact]
    public Task DateTimeOffsetUtcToJsonString()
    {
        var dateTime = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        return Verify(DateFormatter.ToJsonString(new DateTimeOffset(dateTime)));
    }

    [Fact]
    public Task DateTimeOffsetUtcToParameterString()
    {
        var dateTime = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        return Verify(DateFormatter.ToParameterString(new DateTimeOffset(dateTime)));
    }
}