using System.Globalization;

public class IsoDateParseTests
{
    // th-TH defaults to the Buddhist calendar and ar-SA to UmAlQura. Parsing the built in
    // ISO format with either would shift the year or fail outright, so the same input has
    // to scrub to the same value on every machine.
    [Theory]
    [InlineData("th-TH")]
    [InlineData("ar-SA")]
    [InlineData("en-US")]
    public void IsoDateTimeUsesInvariantCalendar(string cultureName)
    {
        using var counter = Counter.Start();

        RunInCulture(
            cultureName,
            () =>
            {
                Assert.True(counter.TryConvertDateTime("9999-12-31T00:00:00".AsSpan(), out var result));
                Assert.Equal("Date_MaxValue", result);
            });
    }

    [Theory]
    [InlineData("th-TH")]
    [InlineData("ar-SA")]
    [InlineData("en-US")]
    public void IsoDateTimeOffsetUsesInvariantCalendar(string cultureName)
    {
        using var counter = Counter.Start();

        RunInCulture(
            cultureName,
            () =>
            {
                Assert.True(counter.TryConvertDateTimeOffset("9999-12-31T00:00:00+00:00".AsSpan(), out var result));
                Assert.Equal("Date_MaxValue", result);
            });
    }

    static void RunInCulture(string cultureName, Action action)
    {
        var original = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new(cultureName);
        try
        {
            action();
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
