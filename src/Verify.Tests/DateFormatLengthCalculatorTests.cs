public class DateFormatLengthCalculatorTests
{
    [Theory]
    [InlineData("y", 4, 1)]
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
    [InlineData("yyyy-MM-dd", 10, 10)]
    public void Combos(string format, int max, int min)
    {
        var length = DateFormatLengthCalculator.GetLength(format.AsSpan(), CultureInfo.InvariantCulture);
        Assert.Equal(max, length.max);
        Assert.Equal(min, length.min);

        var padded = $" {format} ";
        length = DateFormatLengthCalculator.GetLength(padded.AsSpan(), CultureInfo.InvariantCulture);
        Assert.Equal(max + 2, length.max);
        Assert.Equal(min + 2, length.min);

        var prefixed = $" {format}";
        length = DateFormatLengthCalculator.GetLength(prefixed.AsSpan(), CultureInfo.InvariantCulture);
        Assert.Equal(max + 1, length.max);
        Assert.Equal(min + 1, length.min);

        var suffixed = $"{format} ";
        length = DateFormatLengthCalculator.GetLength(suffixed.AsSpan(), CultureInfo.InvariantCulture);
        Assert.Equal(max + 1, length.max);
        Assert.Equal(min + 1, length.min);

        var escaped = $"\\d{format}";
        length = DateFormatLengthCalculator.GetLength(escaped.AsSpan(), CultureInfo.InvariantCulture);
        Assert.Equal(max + 1, length.max);
        Assert.Equal(min + 1, length.min);
    }
}