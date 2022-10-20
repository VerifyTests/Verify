﻿[UsesVerify]
public class DateFormatterTests
{
    [Fact]
    public Task DateTimeOtherTimeZoneToJsonString()
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.FromHours(1.5));
        return Verify(DateFormatter.ToJsonString(date));
    }

    [Fact]
    public Task DateTimeOtherTimeZoneToParameterString()
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.FromHours(1.5));
        return Verify(DateFormatter.ToParameterString(date));
    }

    [Fact]
    public Task DateTimeLocalToJsonString()
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Local);
        return Verify(DateFormatter.ToJsonString(date));
    }

    [Fact]
    public Task DateTimeLocalToParameterString()
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Local);
        return Verify(DateFormatter.ToParameterString(date));
    }

    [Fact]
    public Task DateTimeOffsetLocalToJsonString()
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, DateTimeOffset.Now.Offset);
        return Verify(DateFormatter.ToJsonString(date));
    }

    [Fact]
    public Task DateTimeOffsetLocalToParameterString()
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, DateTimeOffset.Now.Offset);
        return Verify(DateFormatter.ToParameterString(date));
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
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.Zero);
        return Verify(DateFormatter.ToJsonString(date));
    }

    [Fact]
    public Task DateTimeOffsetUtcToParameterString()
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.Zero);
        return Verify(DateFormatter.ToParameterString(date));
    }
}