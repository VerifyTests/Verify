public class DateFormatterTests
{
    [Fact]
    public Task DateTimeOtherTimeZoneToJsonString()
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.FromHours(1.5));
        return Verify(DateFormatter.Convert(date));
    }

    [Fact]
    public Task DateTimeOtherTimeZoneToParameterString()
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.FromHours(1.5));
        return Verify(DateFormatter.ToParameterString(date));
    }

    [Fact]
    public Task DateTimeOtherTimeZoneNegativeToJsonString()
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.FromHours(-1.5));
        return Verify(DateFormatter.Convert(date));
    }

    [Fact]
    public Task DateTimeOtherTimeZoneNegativeToParameterString()
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.FromHours(-1.5));
        return Verify(DateFormatter.ToParameterString(date));
    }

    [Fact]
    public Task DateTimeLocalToJsonString()
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Local);
        return Verify(DateFormatter.Convert(date));
    }

    [Fact]
    public Task DateTimeLocalToParameterString()
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Local);
        return Verify(DateFormatter.ToParameterString(date));
    }

    [Fact]
    public Task DateTimeUnspecifiedToJsonString()
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0);
        return Verify(DateFormatter.Convert(date));
    }

    [Fact]
    public Task DateTimeUnspecifiedToParameterString()
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0);
        return Verify(DateFormatter.ToParameterString(date));
    }

    [Fact]
    public Task DateTimeUtcToJsonString()
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        return Verify(DateFormatter.Convert(date));
    }

    [Fact]
    public Task DateTimeUtcToParameterString()
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        return Verify(DateFormatter.ToParameterString(date));
    }

    [Fact]
    public Task DateTimeOffsetUtcToJsonString()
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.Zero);
        return Verify(DateFormatter.Convert(date));
    }

    [Fact]
    public Task DateTimeOffsetUtcToParameterString()
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.Zero);
        return Verify(DateFormatter.ToParameterString(date));
    }

    [Fact]
    public async Task DateTimeOffsetCombinations()
    {
        var jsonValues = new Dictionary<string, string>();
        var parameterValues = new Dictionary<string, string>();

        foreach (var offset in bools)
        foreach (var hour in bools)
        foreach (var minute in bools)
        foreach (var second in bools)
        foreach (var secondFraction in bools)
        {
            var name = new StringBuilder();
            var timeSpan = TimeSpan.Zero;
            if (offset)
            {
                name.Append("_offset");
                timeSpan = new(7, 8, 0);
            }

            var value = new DateTimeOffset(2020, 1, 1, 0, 0, 0, timeSpan);
            if (hour)
            {
                name.Append("_hour");
                value = value.AddHours(2);
            }

            if (minute)
            {
                name.Append("_minute");
                value = value.AddMinutes(3);
            }

            if (second)
            {
                name.Append("_second");
                value = value.AddSeconds(4);
            }

            if (secondFraction)
            {
                name.Append("_secondFraction");
                value = value.AddSeconds(.5);
            }

            jsonValues.Add(name.ToString(), DateFormatter.Convert(value));
            parameterValues.Add(name.ToString(), DateFormatter.ToParameterString(value));
        }

        await Verify(new
        {
            jsonValues,
            parameterValues
        });
    }

    [Fact]
    public async Task DateTimeCombinations()
    {
        var jsonValues = new Dictionary<string, string>();
        var parameterValues = new Dictionary<string, string>();

        foreach (var kind in new[]
                 {
                     DateTimeKind.Local,
                     DateTimeKind.Unspecified,
                     DateTimeKind.Utc
                 })
        foreach (var hour in bools)
        foreach (var minute in bools)
        foreach (var second in bools)
        foreach (var secondFraction in bools)
        {
            var name = new StringBuilder(kind.ToString());
            var value = new DateTime(2020, 1, 1, 0, 0, 0, kind);
            if (hour)
            {
                name.Append("_hour");
                value = value.AddHours(2);
            }

            if (minute)
            {
                name.Append("_minute");
                value = value.AddMinutes(3);
            }

            if (second)
            {
                name.Append("_second");
                value = value.AddSeconds(4);
            }

            if (secondFraction)
            {
                name.Append("_secondFraction");
                value = value.AddSeconds(.5);
            }

            jsonValues.Add(name.ToString(), DateFormatter.Convert(value));
            parameterValues.Add(name.ToString(), DateFormatter.ToParameterString(value));
        }

        await Verify(new
        {
            jsonValues,
            parameterValues
        });
    }

    [Fact]
    public void SubMillisecondTicksDoNotCollide()
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.NotEqual(
            DateFormatter.ToParameterString(date.AddTicks(1)),
            DateFormatter.ToParameterString(date.AddTicks(2)));

        var offset = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.Zero);
        Assert.NotEqual(
            DateFormatter.ToParameterString(offset.AddTicks(1)),
            DateFormatter.ToParameterString(offset.AddTicks(2)));
    }

    [Fact]
    public Task SubMillisecondTicks()
    {
        var date = new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        var offset = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.Zero);

        var values = new Dictionary<string, object>();
        foreach (var ticks in tickOffsets)
        {
            values.Add(
                ticks.ToString(),
                new
                {
                    dateJson = DateFormatter.Convert(date.AddTicks(ticks)),
                    dateParameter = DateFormatter.ToParameterString(date.AddTicks(ticks)),
                    offsetJson = DateFormatter.Convert(offset.AddTicks(ticks)),
                    offsetParameter = DateFormatter.ToParameterString(offset.AddTicks(ticks))
                });
        }

        return Verify(values);
    }

    static long[] tickOffsets =
    [
        0,
        1,
        2,
        TimeSpan.TicksPerMillisecond,
        TimeSpan.TicksPerSecond,
        TimeSpan.TicksPerSecond + 1,
        TimeSpan.TicksPerMinute,
        TimeSpan.TicksPerMinute + 1
    ];

    static bool[] bools =
    [
        true,
        false
    ];
}