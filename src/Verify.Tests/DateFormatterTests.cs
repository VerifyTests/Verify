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

    // NumberFormatInfo.NegativeSign is not "-" in every culture: sv-SE renders U+2212 and ar-SA
    // prefixes U+061C to it. The offset is both snapshot content and part of a parameter file
    // name, so a negative offset rendered on one machine has to equal the snapshot committed
    // from another. The sign is set explicitly rather than by picking a real culture, so the
    // assertion does not move with the ICU data the test happens to run against
    [Theory]
    [InlineData(-1.5, "2000-10-01 -1-30", "2000-10-01-1-30")]
    [InlineData(-5, "2000-10-01 -5", "2000-10-01-5")]
    [InlineData(1.5, "2000-10-01 +1-30", "2000-10-01+1-30")]
    [InlineData(5, "2000-10-01 +5", "2000-10-01+5")]
    [InlineData(0, "2000-10-01 +0", "2000-10-01+0")]
    public void OffsetDoesNotTakeTheCurrentCultureNegativeSign(double hours, string expectedJson, string expectedParameter)
    {
        var date = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.FromHours(hours));

        // U+2212 MINUS SIGN, as sv-SE uses
        var culture = (CultureInfo) CultureInfo.InvariantCulture.Clone();
        culture.NumberFormat.NegativeSign = "−";

        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = culture;

            Assert.Equal(expectedJson, DateFormatter.Convert(date));
            Assert.Equal(expectedParameter, DateFormatter.ToParameterString(date));
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
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

    // A sub hour offset has a zero hour component, so the sign has to come from
    // the offset itself
    [Fact]
    public void SubHourOffsets()
    {
        var negative = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.FromMinutes(-30));
        Assert.Equal("2000-10-01 -0-30", DateFormatter.Convert(negative));
        Assert.Equal("2000-10-01-0-30", DateFormatter.ToParameterString(negative));

        var positive = new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.FromMinutes(30));
        Assert.Equal("2000-10-01 +0-30", DateFormatter.Convert(positive));
        Assert.Equal("2000-10-01+0-30", DateFormatter.ToParameterString(positive));

        Assert.NotEqual(
            DateFormatter.ToParameterString(negative),
            DateFormatter.ToParameterString(positive));
    }

    static bool[] bools =
    [
        true,
        false
    ];
}