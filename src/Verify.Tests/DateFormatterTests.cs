[UsesVerify]
public class DateFormatterTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task DateTimeOtherTimeZoneToJsonString(bool offset)
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.FromHours(1.5));
        return Verify(DateFormatter.ToJsonString(date, offset))
            .UseParameters( offset);
    }

    [Theory]
    [InlineData( true)]
    [InlineData( false)]
    public Task DateTimeOtherTimeZoneToParameterString(bool offset)
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.FromHours(1.5));
        return Verify(DateFormatter.ToParameterString(date, offset))
            .UseParameters(offset);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task DateTimeLocalToJsonString(bool kind)
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Local);
        return Verify(DateFormatter.ToJsonString(date, kind))
            .UseParameters(kind);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task DateTimeLocalToParameterString(bool kind)
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Local);
        return Verify(DateFormatter.ToParameterString(date, kind))
            .UseParameters(kind);
    }

    [Theory]
    [InlineData(true)]
    public Task DateTimeOffsetLocalToJsonString(bool offset)
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, DateTimeOffset.Now.Offset);
        return Verify(DateFormatter.ToJsonString(date, offset))
            .UseParameters(offset);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task DateTimeOffsetLocalToParameterString(bool offset)
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, DateTimeOffset.Now.Offset);
        return Verify(DateFormatter.ToParameterString(date, offset))
            .UseParameters(offset);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task DateTimeUnspecifiedToJsonString(bool kind)
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0);
        return Verify(DateFormatter.ToJsonString(date, kind))
            .UseParameters(kind);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task DateTimeUnspecifiedToParameterString(bool kind)
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Unspecified);
        return Verify(DateFormatter.ToParameterString(date, kind))
            .UseParameters(kind);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task DateTimeUtcToJsonString(bool kind)
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        return Verify(DateFormatter.ToJsonString(date, kind))
            .UseParameters(kind);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task DateTimeUtcToParameterString(bool kind)
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        return Verify(DateFormatter.ToParameterString(date, kind))
            .UseParameters(kind);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task DateTimeOffsetUtcToJsonString(bool offset)
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.Zero);
        return Verify(DateFormatter.ToJsonString(date, offset))
            .UseParameters(offset);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task DateTimeOffsetUtcToParameterString(bool offset)
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.Zero);
        return Verify(DateFormatter.ToParameterString(date, offset))
            .UseParameters(offset);
    }
}