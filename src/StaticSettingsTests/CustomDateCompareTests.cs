public class CustomDateCompareTests :
    BaseTest
{
    #region CustomDateTimeComparer

    [ModuleInitializer]
    public static void UseCustomDateTimeComparer() =>
        Counter.UseDateTimeComparer(new CustomDateTimeComparer());

    public class CustomDateTimeComparer :
        IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime x, DateTime y) =>
            new DateTime(x.Year, x.Month, x.Day) ==
            new DateTime(y.Year, y.Month, y.Day);

        public int GetHashCode(DateTime date) =>
            new DateTime(date.Year, date.Month, date.Day).GetHashCode();
    }

    #endregion

    #region CustomTimeComparer

    [ModuleInitializer]
    public static void UseCustomTimeComparer() =>
        Counter.UseTimeComparer(new CustomTimeComparer());

    public class CustomTimeComparer :
        IEqualityComparer<Time>
    {
        public bool Equals(Time x, Time y) =>
            new Time(x.Hour, x.Minute, x.Second) ==
            new Time(y.Hour, y.Minute, y.Second);

        public int GetHashCode(Time date) =>
            new Time(date.Hour, date.Minute, date.Second).GetHashCode();
    }

    #endregion

    #region CustomDateTimeOffsetComparer

    [ModuleInitializer]
    public static void UseCustomDateTimeOffsetComparer() =>
        Counter.UseDateTimeOffsetComparer(new CustomDateTimeOffsetComparer());

    public class CustomDateTimeOffsetComparer :
        IEqualityComparer<DateTimeOffset>
    {
        public bool Equals(DateTimeOffset x, DateTimeOffset y) =>
            new DateTimeOffset(new(x.Year, x.Month, x.Day)) ==
            new DateTimeOffset(new(y.Year, y.Month, y.Day));

        public int GetHashCode(DateTimeOffset date)
        {
            var dateTime = new DateTime(date.Year, date.Month, date.Day);
            return new DateTimeOffset(dateTime)
                .GetHashCode();
        }
    }

    #endregion

    public CustomDateCompareTests()
    {
        Counter.UseDateTimeComparer(new Compare());
        Counter.UseDateTimeOffsetComparer(new Compare());
        Counter.UseTimeComparer(new Compare());
    }

    [Fact]
    public Task Run() =>
        Verify(new
        {
            Time1 = new Time(1, 2),
            Time2 = new Time(3, 4),
            DateTime1 = DateTime.Now,
            DateTime2 = DateTime.Now.AddDays(2),
            DateTimeOffset1 = new DateTimeOffset(DateTime.Now),
            DateTimeOffset2 = new DateTimeOffset(DateTime.Now.AddDays(2)),
        });

    public class Compare :
        IEqualityComparer<DateTime>,
        IEqualityComparer<DateTimeOffset>,
        IEqualityComparer<Time>
    {
        public bool Equals(DateTime x, DateTime y) =>
            true;

        public int GetHashCode(DateTime obj) =>
            0;

        public bool Equals(DateTimeOffset x, DateTimeOffset y) =>
            true;

        public int GetHashCode(DateTimeOffset obj) =>
            0;

        public bool Equals(Time x, Time y) =>
            true;

        public int GetHashCode(Time obj) =>
            0;
    }
}