public class CounterComparersTests
{
    private class StubGuidComparer : IEqualityComparer<Guid>
    {
        public bool Equals(Guid x, Guid y) => true;

        public int GetHashCode(Guid obj) => 1;
    }

    [Fact]
    public async Task ShouldInjectCustomGuidComparer()
    {
        var obj = new
        {
            Item1 = Guid.NewGuid(),
            Item2 = Guid.NewGuid(),
            Item3 = Guid.NewGuid()
        };

        var settings = new VerifySettings();
        settings.ReplaceScrubberGuidComparer(new StubGuidComparer());
        await Verify(obj, settings: settings);
    }

#if NET6_0_OR_GREATER
    private class StubDateComparer : IEqualityComparer<Date>
    {
        public bool Equals(Date x, Date y) => x.Year == y.Year;

        public int GetHashCode(Date obj) => 1;
    }

    [Fact]
    public async Task ShouldInjectCustomDateComparer()
    {
        var obj = new
        {
            Item1 = new Date(1999, 5, 1),
            Item2 = new Date(1999, 12, 4),
            Item3 = new Date(2666, 5, 1),
        };

        var settings = new VerifySettings();
        settings.ReplaceScrubberDateComparer(new StubDateComparer());
        await Verify(obj, settings: settings);
    }

    private class StubTimeComparer : IEqualityComparer<Time>
    {
        public bool Equals(Time x, Time y) => x.Hour == y.Hour;

        public int GetHashCode(Time obj) => 1;
    }

    [Fact]
    public async Task ShouldInjectCustomTimeComparer()
    {
        var obj = new
        {
            Item1 = new Time(23, 5, 1),
            Item2 = new Time(23, 1, 1),
            Item3 = new Time(1, 1, 1),
            Item4 = Guid.NewGuid(),
            Item5 = Guid.NewGuid()
        };

        var settings = new VerifySettings();
        settings.ReplaceScrubberTimeComparer(new StubTimeComparer());
        await Verify(obj, settings: settings);
    }
#endif

    private class StubDateTimeComparer : IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime x, DateTime y) => x.Year == y.Year;

        public int GetHashCode(DateTime obj) => 1;
    }

    [Fact]
    public async Task ShouldInjectCustomDateTimeComparer()
    {
        var obj = new
        {
            Item1 = new DateTime(23, 5, 1),
            Item2 = new DateTime(23, 1, 1),
            Item3 = new DateTime(1, 1, 1),
            Item4 = Guid.NewGuid(),
            Item5 = Guid.NewGuid()
        };

        var settings = new VerifySettings();
        settings.ReplaceScrubberDateTimeComparer(new StubDateTimeComparer());
        await Verify(obj, settings: settings);
    }

    private class StubDateTimeOffsetComparer : IEqualityComparer<DateTimeOffset>
    {
        public bool Equals(DateTimeOffset x, DateTimeOffset y) => x.Second == y.Second;

        public int GetHashCode(DateTimeOffset obj) => 1;
    }

    [Fact]
    public async Task ShouldInjectCustomDateTimeOffsetComparer()
    {
        var obj = new
        {
            Item1 = new DateTimeOffset(2021, 1, 1, 7, 1, 1, TimeSpan.FromHours(1)),
            Item2 = new DateTimeOffset(2023, 5, 1, 1, 1, 1, TimeSpan.FromHours(1)),
            Item3 = new DateTimeOffset(2024, 1, 6, 1, 2, 5, TimeSpan.FromHours(1)),
            Item4 = Guid.NewGuid(),
            Item5 = Guid.NewGuid()
        };

        var settings = new VerifySettings();
        settings.ReplaceScrubberDateTimeOffsetComparer(new StubDateTimeOffsetComparer());
        await Verify(obj, settings: settings);
    }

    [Fact]
    public async Task ShouldInjectMultipleComparers()
    {
        var obj = new
        {
            Item1 = new DateTimeOffset(2021, 1, 1, 7, 1, 1, TimeSpan.FromHours(1)),
            Item2 = new DateTimeOffset(2023, 5, 1, 1, 1, 1, TimeSpan.FromHours(1)),
            Item3 = new DateTimeOffset(2024, 1, 6, 1, 2, 1, TimeSpan.FromHours(1)),
            Item4 = Guid.NewGuid(),
            Item5 = Guid.NewGuid()
        };

        var settings = new VerifySettings();
        settings.ReplaceScrubberDateTimeOffsetComparer(new StubDateTimeOffsetComparer());
        settings.ReplaceScrubberGuidComparer(new StubGuidComparer());
        await Verify(obj, settings: settings);
    }
}