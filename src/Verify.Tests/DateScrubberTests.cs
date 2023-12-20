#if NET6_0_OR_GREATER

[UsesVerify]
public class DateScrubberTests
{
    #region NamedDate

    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.AddNamedDate(new(1998, 10, 1), "dateName");

    #endregion

    [Theory]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "no match")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaa", "no match short")]
    [InlineData("1995-10-01", "simple")]
    [InlineData("a1995-10-01b", "wrapped")]
    [InlineData("1995-10-01b", "trailing")]
    [InlineData("a1995-10-01", "starting")]
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
    [InlineData("yyyy-MM-dd")]
    [InlineData("yyyy MMM ddd")]
    [InlineData("yyyy MMMM dddd")]
    [InlineData("yyyy-MM-d")]
    [InlineData("yyyy-M-dd")]
    [InlineData("yyyy-M-d")]
    [InlineData("y-M-d")]
    public Task Lengths(string format) =>
        Verify(DateScrubber.Lengths(format, CultureInfo.InvariantCulture))
            .UseTextForParameters(format);
    [Theory]
    [InlineData("yyyy-MM-dd")]
    [InlineData("yyyy MMM ddd")]
    [InlineData("yyyy MMMM dddd")]
    [InlineData("yyyy-MM-d")]
    [InlineData("yyyy-M-dd")]
    [InlineData("yyyy-M-d")]
    [InlineData("y-M-d")]
    public Task LengthsGerman(string format) =>
        Verify(DateScrubber.Lengths(format,CultureInfo.GetCultureInfo("de-DE")))
            .UseTextForParameters(format);

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

    [Fact]
    public Task AllCultures()
    {
        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
        return Verify(cultures
            .Select(_ => _.DateTimeFormat.AMDesignator + _.DateTimeFormat.PMDesignator)
            .OrderBy(_ => _.Length));
    }
    [Fact]
    public Task LongestDayName() =>
        Verify(CultureInfo.InvariantCulture.DateTimeFormat.LongestDayName());
    [Fact]
    public Task LongestMonthName()
    {
        var dateTimeFormat = CultureInfo.InvariantCulture.DateTimeFormat;

        var monthNumber = dateTimeFormat.LongestMonthName();
        var monthName = dateTimeFormat.GetMonthName(monthNumber);
        return Verify(new
        {
            monthNumber,
            monthName
        });
    }

    const string monthDayFormat = "MMMM MMM dddd ddd HH:mm:ss";

    [Fact]
    public Task BuildCultureToDate()
    {
        var map = new Dictionary<CultureInfo, CultureDates>();
        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
        foreach (var culture in cultures)
        {
            map[culture] = new()
            {
                Long = FindLongDate(culture),
                Short = FindShortDate(culture),
            };
        }
        return Verify(map).DontScrubDateTimes();
    }

    static DateTimeOffset FindLongDate(CultureInfo culture)
    {
        DateTimeOffset longDate = default;
        var longFormatted = "";
        var formatInfo = culture.DateTimeFormat;
        var amLength = formatInfo.AMDesignator.Length;
        var pmLength = formatInfo.PMDesignator.Length;
        for (var month = 1; month <= 12; month++)
        {
            for (var day = 20; day <= 27; day++)
            {
                DateTimeOffset date;
                if (amLength > pmLength)
                {
                    date = new(2023, month, day, 1, 0, 0, 0, TimeSpan.Zero);
                }
                else
                {
                    date = new(2023, month, day, 13, 0, 0, 0, TimeSpan.Zero);
                }

                var formatted = date.ToString(monthDayFormat, formatInfo);
                if (formatted.Length > longFormatted.Length)
                {
                    longFormatted = formatted;
                    longDate = date;
                }
            }
        }

        return longDate;
    }

    static DateTimeOffset FindShortDate(CultureInfo culture)
    {
        DateTimeOffset shortDate = default;
        string? shortFormatted = null;
        var formatInfo = culture.DateTimeFormat;
        var amLength = formatInfo.AMDesignator.Length;
        var pmLength = formatInfo.PMDesignator.Length;
        for (var month = 1; month <= 12; month++)
        {
            for (var day = 1; day <= 7; day++)
            {
                DateTimeOffset date;
                if (amLength < pmLength)
                {
                    date = new(2023, month, day, 1, 0, 0, 0, TimeSpan.Zero);
                }
                else
                {
                    date = new(2023, month, day, 13, 0, 0, 0, TimeSpan.Zero);
                }

                var formatted = date.ToString(monthDayFormat, formatInfo);
                if (shortFormatted == null ||
                    formatted.Length < shortFormatted.Length)
                {
                    shortFormatted = formatted;
                    shortDate = date;
                }
            }
        }

        return shortDate;
    }

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