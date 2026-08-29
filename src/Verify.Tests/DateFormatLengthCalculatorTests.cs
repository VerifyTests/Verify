public class DateFormatLengthCalculatorTests
{
    [Theory]
    [InlineData("y", 4, 1)]
    [InlineData("%y", 4, 1)]
    [InlineData("yy", 4, 2)]
    [InlineData("yyy", 4, 3)]
    [InlineData("yyyy", 4, 4)]
    [InlineData("yyyyy", 5, 5)]
    [InlineData("M", 2, 1)]
    [InlineData("MM", 2, 2)]
    [InlineData("MMM", 3, 3)]
    [InlineData("MMMM", 9, 3)]
    [InlineData("MMMMM", 9, 3)]
    [InlineData("d", 2, 1)]
    [InlineData("dd", 2, 2)]
    [InlineData("ddd", 3, 3)]
    [InlineData("dddd", 9, 6)]
    [InlineData("ddddd", 9, 6)]
    [InlineData("h", 2, 1)]
    [InlineData("hh", 2, 2)]
    [InlineData("hhh", 2, 2)]
    [InlineData("m", 2, 1)]
    [InlineData("mm", 2, 2)]
    [InlineData("mmm", 2, 2)]
    [InlineData("s", 2, 1)]
    [InlineData("ss", 2, 2)]
    [InlineData("sss", 2, 2)]
    [InlineData("f", 1, 1)]
    [InlineData("ff", 2, 2)]
    [InlineData("fff", 3, 3)]
    [InlineData("ffff", 4, 4)]
    [InlineData("fffff", 5, 5)]
    [InlineData("ffffff", 6, 6)]
    [InlineData("fffffff", 7, 7)]
    [InlineData("g", 4, 4)]
    [InlineData("gg", 4, 4)]
    [InlineData("ggg", 4, 4)]
    // A single t renders only the first character of the AM/PM designator
    [InlineData("t", 1, 1)]
    [InlineData("tt", 2, 2)]
    [InlineData("ttt", 2, 2)]
    [InlineData("z", 3, 2)]
    [InlineData("zz", 3, 3)]
    [InlineData("zzz", 6, 6)]
    [InlineData("zzzz", 6, 6)]
    // K renders as "" (Unspecified), "Z" (Utc, 1 char) or "+11:00" (offset, 6 chars),
    // so its minimum contribution is 0 (not 6) — otherwise round-trip/"o" formats
    // scrub only the offset form and leak the Z / offset-less forms.
    [InlineData("K", 6, 0)]
    [InlineData("KK", 12, 0)]
    [InlineData(":", 1, 1)]
    [InlineData("':'", 1, 1)]
    // The escape backslash inside a quoted literal is consumed, not rendered
    [InlineData(@"'o\'clock'", 7, 7)]
    [InlineData("/", 1, 1)]
    [InlineData("'/'", 1, 1)]
    [InlineData("yyyy-MM-dd", 10, 10)]
    [InlineData("yyyy/MM/dd", 10, 10)]
    [InlineData("yyyy'/'MM'/'dd", 10, 10)]
    [InlineData("yyyy-MM-ddTHH:mm:ss.FFFF", 24, 19)]
    [InlineData("yyyy-MM-ddTHH:mm:ss.F", 21, 19)]
    public void Combos(string format, int max, int min)
    {
        var culture = CultureInfo.InvariantCulture;

        var length = DateFormatLengthCalculator.InnerGetLength(format.AsSpan(), culture);
        Assert.Equal(max, length.max);
        Assert.Equal(min, length.min);

        if (format.Length > 1)
        {
            var result = DateTime.Now.ToString(format, culture);
            Assert.True(result.Length <= max, $"{result.Length} <= {max}. {result}");
            Assert.True(result.Length >= min, $"{result.Length} >= {min}. {result}");
        }

        var padded = $" {format} ";
        length = DateFormatLengthCalculator.InnerGetLength(padded.AsSpan(), culture);
        Assert.Equal(max + 2, length.max);
        Assert.Equal(min + 2, length.min);

        var prefixed = $" {format}";
        length = DateFormatLengthCalculator.InnerGetLength(prefixed.AsSpan(), culture);
        Assert.Equal(max + 1, length.max);
        Assert.Equal(min + 1, length.min);

        var suffixed = $"{format} ";
        length = DateFormatLengthCalculator.InnerGetLength(suffixed.AsSpan(), culture);
        Assert.Equal(max + 1, length.max);
        Assert.Equal(min + 1, length.min);

        var escapedPrefixed = $@"\d{format}";
        length = DateFormatLengthCalculator.InnerGetLength(escapedPrefixed.AsSpan(), culture);
        Assert.Equal(max + 1, length.max);
        Assert.Equal(min + 1, length.min);

        var escapedSuffixed = $@"{format}\d";
        length = DateFormatLengthCalculator.InnerGetLength(escapedSuffixed.AsSpan(), culture);
        Assert.Equal(max + 1, length.max);
        Assert.Equal(min + 1, length.min);

        var escapedWrapped = $@"\d{format}\d";
        length = DateFormatLengthCalculator.InnerGetLength(escapedWrapped.AsSpan(), culture);
        Assert.Equal(max + 2, length.max);
        Assert.Equal(min + 2, length.min);
    }

    // Two cultures can share a name and still render dates differently, so the cache
    // cannot be keyed on the name: whichever was measured first would supply the bounds
    // for the other, and rendered dates would fall outside the probed window lengths.
    [Fact]
    public void SameNamedCulturesWithDifferentFormatsAreNotShared()
    {
        var standard = new CultureInfo("en-AU");
        var customized = new CultureInfo("en-AU")
        {
            DateTimeFormat =
            {
                // A far longer designator, so the bounds cannot coincide by accident
                PMDesignator = "in the afternoon",
                AMDesignator = "in the morning"
            }
        };

        var standardLength = DateFormatLengthCalculator.GetLength("h:mm tt", standard);
        var customizedLength = DateFormatLengthCalculator.GetLength("h:mm tt", customized);

        Assert.NotEqual(standardLength, customizedLength);

        var rendered = new DateTime(2020, 1, 1, 13, 30, 0).ToString("h:mm tt", customized);
        Assert.True(rendered.Length <= customizedLength.max, $"{rendered.Length} <= {customizedLength.max}. {rendered}");
        Assert.True(rendered.Length >= customizedLength.min, $"{rendered.Length} >= {customizedLength.min}. {rendered}");
    }

    // MMMM next to a day component renders the genitive month name, which can be longer
    // than every nominative form (cs-CZ November: "listopadu" vs "listopad")
    [Fact]
    public void GenitiveMonthNamesFeedTheBounds()
    {
        var culture = new CultureInfo("cs-CZ");
        var (max, min) = DateFormatLengthCalculator.InnerGetLength("d MMMM yyyy".AsSpan(), culture);
        var rendered = new DateTime(2020, 11, 15).ToString("d MMMM yyyy", culture);
        Assert.True(rendered.Length <= max, $"{rendered.Length} <= {max}. {rendered}");
        Assert.True(rendered.Length >= min, $"{rendered.Length} >= {min}. {rendered}");
    }
}