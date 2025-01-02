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
    [InlineData("t", 1, 1)]
    [InlineData("tt", 2, 2)]
    [InlineData("ttt", 2, 2)]
    [InlineData("z", 3, 2)]
    [InlineData("zz", 3, 3)]
    [InlineData("zzz", 6, 6)]
    [InlineData("zzzz", 6, 6)]
    [InlineData("K", 6, 6)]
    [InlineData("KK", 12, 12)]
    [InlineData(":", 1, 1)]
    [InlineData("':'", 1, 1)]
    [InlineData("/", 1, 1)]
    [InlineData("'/'", 1, 1)]
    [InlineData("yyyy-MM-dd", 10, 10)]
    [InlineData("yyyy/MM/dd", 10, 10)]
    [InlineData("yyyy'/'MM'/'dd", 10, 10)]
    public void Combos(string format, int max, int min)
    {
        var culture = CultureInfo.InvariantCulture;
        //var s = DateTime.Now.ToString(format,culture);
        var length = DateFormatLengthCalculator.GetLength(format.AsSpan(), culture);
        Assert.Equal(max, length.max);
        Assert.Equal(min, length.min);

        var padded = $" {format} ";
        length = DateFormatLengthCalculator.GetLength(padded.AsSpan(), culture);
        Assert.Equal(max + 2, length.max);
        Assert.Equal(min + 2, length.min);

        var prefixed = $" {format}";
        length = DateFormatLengthCalculator.GetLength(prefixed.AsSpan(), culture);
        Assert.Equal(max + 1, length.max);
        Assert.Equal(min + 1, length.min);

        var suffixed = $"{format} ";
        length = DateFormatLengthCalculator.GetLength(suffixed.AsSpan(), culture);
        Assert.Equal(max + 1, length.max);
        Assert.Equal(min + 1, length.min);

        var escapedPrefixed = $@"\d{format}";
        length = DateFormatLengthCalculator.GetLength(escapedPrefixed.AsSpan(), culture);
        Assert.Equal(max + 1, length.max);
        Assert.Equal(min + 1, length.min);

        var escapedSuffixed = $@"{format}\d";
        length = DateFormatLengthCalculator.GetLength(escapedSuffixed.AsSpan(), culture);
        Assert.Equal(max + 1, length.max);
        Assert.Equal(min + 1, length.min);

        var escapedWrapped = $@"\d{format}\d";
        length = DateFormatLengthCalculator.GetLength(escapedWrapped.AsSpan(), culture);
        Assert.Equal(max + 2, length.max);
        Assert.Equal(min + 2, length.min);
    }
}