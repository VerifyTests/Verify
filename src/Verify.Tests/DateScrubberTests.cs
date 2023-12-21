[UsesVerify]
public class DateScrubberTests
{
    #region NamedDate

    [ModuleInitializer]
    public static void Init()
    {
#if NET6_0_OR_GREATER
        VerifierSettings.AddNamedDate(new(1998, 10, 1), "dateName");
#endif
        VerifierSettings.AddNamedDateTime(new(1998, 10, 1), "dateTimeName");
        VerifierSettings.AddNamedDateTimeOffset(new(new(1998, 10, 1)), "dateTimeOffsetName");
    }

    #endregion

#if NET6_0_OR_GREATER
    [Theory]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "no match")]
    [InlineData("aaaa", "no match short")]
    [InlineData("1995-10-01", "simple")]
    [InlineData("a1995-10-01b", "wrapped")]
    [InlineData("1995-10-01b", "trailing")]
    [InlineData("a1995-10-01", "starting")]
    [InlineData("1995-10-01 1995-10-01 1995-10-02", "multiple")]
    [InlineData("1998-10-01", "named")]
    public async Task Dates(string value, string name)
    {
        var counter = Counter.Start();
        try
        {
            var builder = new StringBuilder(value);
            DateScrubber.ReplaceDates(builder, "yyyy-MM-dd", counter, CultureInfo.InvariantCulture);
            await Verify(builder)
                .UseTextForParameters(name);
        }
        finally
        {
            Counter.Stop();
        }
    }
    [Theory]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "no match")]
    [InlineData("aaaa", "no match short")]
    [InlineData("2023 December 21 Thursday", "simple")]
    [InlineData("a2023 December 21 Thursdayb", "wrapped")]
    [InlineData("2023 December 21 Thursdayb", "trailing")]
    [InlineData("a2023 December 21 Thursday", "starting")]
    [InlineData("2023 December 21 Thursday 2023 December 21 Thursday 2023 December 22 Friday", "multiple")]
    [InlineData("1998 October 01 Thursday", "named")]
    public async Task VariableLengthDates(string value, string name)
    {
        var counter = Counter.Start();
        try
        {
            var builder = new StringBuilder(value);
            DateScrubber.ReplaceDates(builder, "yyyy MMMM dd dddd", counter, CultureInfo.InvariantCulture);
            await Verify(builder)
                .UseTextForParameters(name);
        }
        finally
        {
            Counter.Stop();
        }
    }

    [Theory]
    [InlineData("1998-10-01", "named")]
    [InlineData("1935-10-01", "instanceNamed")]
    public Task NamedDates(string value, string name) =>
        Verify(
                new
                {
                    value = Date.Parse(value)
                })
            .AddNamedDate(new(1935, 10, 1), "instanceNamed")
            .UseTextForParameters(name);


    #region InstanceNamedDate

    [Fact]
    public Task InstanceNamedDate() =>
        Verify(
                new
                {
                    value = new Date(1935, 10, 1)
                })
            .AddNamedDate(new(1935, 10, 1), "instanceNamed");

    #endregion
#endif

    #region InstanceNamedDateTime

    [Fact]
    public Task InstanceNamedDateTime() =>
        Verify(
                new
                {
                    value = new DateTime(1935, 10, 1)
                })
            .AddNamedDateTime(new(1935, 10, 1), "instanceNamed");

    #endregion

    #region InstanceNamedDateTimeOffset

    [Fact]
    public Task InstanceNamedDateTimeOffset() =>
        Verify(
                new
                {
                    value = new DateTimeOffset(new(1935, 10, 1))
                })
            .AddNamedDateTimeOffset(new(new(1935, 10, 1)), "instanceNamed");

    #endregion
}