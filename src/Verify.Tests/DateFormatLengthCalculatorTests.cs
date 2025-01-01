public class DateFormatLengthCalculatorTests
{
    [Theory]
    [InlineData("y", 4, 1)]
    [InlineData("yy", 4, 2)]
    [InlineData("yyy", 4, 3)]
    [InlineData("yyyy", 4, 4)]
    [InlineData("yyyyy", 5, 5)]
    [InlineData("d", 2, 1)]
    [InlineData("dd", 2, 2)]
    [InlineData("ddd", 3, 3)]
    [InlineData("dddd", 9, 4)]
    [InlineData("ddddd", 9, 5)]
    [InlineData("yyyy-MM-dd", 10, 10)]
    public void Combos(string format, int max, int min)
    {
        var length = DateFormatLengthCalculator.GetLength(format.AsSpan(), CultureInfo.InvariantCulture);
        Assert.Equal(max, length.max);
        Assert.Equal(min, length.min);
    }
}