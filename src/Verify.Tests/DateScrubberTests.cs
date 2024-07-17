// ReSharper disable StringLiteralTypo
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

    [Fact]
    public Task GetCultureDates() =>
        Verify(
            new
            {
                invarient = DateScrubber.GetCultureDates(CultureInfo.InvariantCulture),
                parent = DateScrubber.GetCultureDates(CultureInfo.GetCultureInfo("de")),
                child = DateScrubber.GetCultureDates(CultureInfo.GetCultureInfo("de-DE"))
            });

    [Theory]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "no match")]
    [InlineData("aaaa", "no match short")]
    [InlineData("1995-10-01", "simple")]
    [InlineData("a1995-10-01b", "wrapped")]
    [InlineData("1995-10-01b", "trailing")]
    [InlineData("a1995-10-01", "starting")]
    [InlineData("1995-10-01 1995-10-01 1995-10-02", "multiple")]
    [InlineData("1998-10-01", "named")]
    public async Task DateTimeOffsets(string value, string name)
    {
        var counter = Counter.Start();
        try
        {
            var builder = new StringBuilder(value);
            DateScrubber.ReplaceDateTimeOffsets(builder, "yyyy-MM-dd", counter, CultureInfo.InvariantCulture);
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
    public async Task VariableLengthDateTimeOffsets(string value, string name)
    {
        var counter = Counter.Start();
        try
        {
            var builder = new StringBuilder(value);
            DateScrubber.ReplaceDateTimeOffsets(builder, "yyyy MMMM dd dddd", counter, CultureInfo.InvariantCulture);
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
    public Task NamedDateTimeOffsets(string value, string name) =>
        Verify(
                new
                {
                    value = DateTimeOffset.Parse(value)
                })
            .AddNamedDateTimeOffset(new(new(1935, 10, 1)), "instanceNamed")
            .UseTextForParameters(name);

    [Theory]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "no match")]
    [InlineData("aaaa", "no match short")]
    [InlineData("1995-10-01", "simple")]
    [InlineData("a1995-10-01b", "wrapped")]
    [InlineData("1995-10-01b", "trailing")]
    [InlineData("a1995-10-01", "starting")]
    [InlineData("1995-10-01 1995-10-01 1995-10-02", "multiple")]
    [InlineData("1998-10-01", "named")]
    public async Task DateTimes(string value, string name)
    {
        var counter = Counter.Start();
        try
        {
            var builder = new StringBuilder(value);
            DateScrubber.ReplaceDateTimes(builder, "yyyy-MM-dd", counter, CultureInfo.InvariantCulture);
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
    public async Task VariableLengthDateTimes(string value, string name)
    {
        var counter = Counter.Start();
        try
        {
            var builder = new StringBuilder(value);
            DateScrubber.ReplaceDateTimes(builder, "yyyy MMMM dd dddd", counter, CultureInfo.InvariantCulture);
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
    public Task NamedDateTimes(string value, string name) =>
        Verify(
                new
                {
                    value = DateTime.Parse(value)
                })
            .AddNamedDateTime(new(1935, 10, 1), "instanceNamed")
            .UseTextForParameters(name);

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

    #region NamedDateInstance

    [Fact]
    public Task NamedDateInstance()
    {
        var dateOnly = new Date(1935, 10, 1);
        var settings = new VerifySettings();
        settings.AddNamedDate(dateOnly, "instanceNamed");
        return Verify(
            new
            {
                value = dateOnly
            },
            settings);
    }

    #endregion

    #region NamedDateFluent

    [Fact]
    public Task NamedDateFluent()
    {
        var dateOnly = new Date(1935, 10, 1);
        return Verify(
                new
                {
                    value = dateOnly
                })
            .AddNamedDate(dateOnly, "instanceNamed");
    }

    #endregion

#endif

    #region NamedDateTimeInstance

    [Fact]
    public Task NamedDateTimeInstance()
    {
        var settings = new VerifySettings();
        var dateTime = new DateTime(1935, 10, 1);
        settings.AddNamedDateTime(dateTime, "instanceNamed");
        return Verify(
            new
            {
                value = dateTime
            },
            settings);
    }

    #endregion

    //top level should not scrub
    [Fact]
    public Task NamedDateTimeTopLevelInstance()
    {
        var settings = new VerifySettings();
        var dateTime = new DateTime(1935, 10, 1);
        settings.AddNamedDateTime(dateTime, "instanceNamed");
        return Verify(dateTime, settings);
    }

    #region NamedDateTimeFluent

    [Fact]
    public Task NamedDateTimeFluent() =>
        Verify(
                new
                {
                    value = new DateTime(1935, 10, 1)
                })
            .AddNamedDateTime(new(1935, 10, 1), "instanceNamed");

    #endregion

    [Fact]
    public Task NamedDateTimeTopLevelFluent()
    {
        var dateTime = new DateTime(1935, 10, 1);
        return Verify(dateTime)
            .AddNamedDateTime(dateTime, "instanceNamed");
    }

    #region NamedDateTimeOffsetInstance

    [Fact]
    public Task NamedDateTimeOffsetInstance()
    {
        var settings = new VerifySettings();
        var dateTimeOffset = new DateTimeOffset(new(1935, 10, 1));
        settings.AddNamedDateTimeOffset(dateTimeOffset, "instanceNamed");
        return Verify(
            new
            {
                value = dateTimeOffset
            },
            settings);
    }

    #endregion

    //top level should not scrub
    [Fact]
    public Task NamedDateTimeOffsetTopLevelInstance()
    {
        var settings = new VerifySettings();
        var dateTimeOffset = new DateTimeOffset(new(1935, 10, 1));
        settings.AddNamedDateTimeOffset(dateTimeOffset, "instanceNamed");
        return Verify(dateTimeOffset, settings);
    }

    #region NamedDateTimeOffsetFluent

    [Fact]
    public Task NamedDateTimeOffsetFluent()
    {
        var dateTimeOffset = new DateTimeOffset(new(1935, 10, 1));
        return Verify(
                new
                {
                    value = dateTimeOffset
                })
            .AddNamedDateTimeOffset(dateTimeOffset, "instanceNamed");
    }

    #endregion

    [Fact]
    public Task NamedDateTimeOffsetTopLevelFluent()
    {
        var dateTimeOffset = new DateTimeOffset(new(1935, 10, 1));
        return Verify(dateTimeOffset)
            .AddNamedDateTimeOffset(dateTimeOffset, "instanceNamed");
    }

    [Fact]
    public Task NamedDateTimeOffsetTopLevelGlobal() =>
        Verify(new DateTimeOffset(new(1998, 10, 1)));
}