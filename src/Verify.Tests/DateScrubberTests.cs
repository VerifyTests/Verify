#if NET6_0_OR_GREATER

[UsesVerify]
public class DateScrubberTests
{
    #region NamedGuid

    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.AddNamedDate(new(1998, 10, 1), "dateName");

    #endregion

    [Theory]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "no match")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaa", "no match short")]
    [InlineData("1995-10-01", "simple")]
    public async Task Dates(string value, string name)
    {
        var counter = Counter.Start();
        try
        {
            var builder = new StringBuilder(value);
            DateScrubber.ReplaceDates(builder, counter);
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
}
#endif