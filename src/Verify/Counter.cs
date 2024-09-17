namespace VerifyTests;

public partial class Counter
{
#if NET6_0_OR_GREATER
    Dictionary<Date, string> namedDates;
    Dictionary<Time, string> namedTimes;
#endif
    Dictionary<DateTime, string> namedDateTimes;
    Dictionary<Guid, string> namedGuids;
    Dictionary<DateTimeOffset, string> namedDateTimeOffsets;
    bool dateCounting;
    static AsyncLocal<Counter?> local = new();

    public static Counter Current
    {
        get
        {
            var context = local.Value;
            if (context is null)
            {
                throw new("No current context");
            }

            return context;
        }
    }

    Counter(
        bool dateCounting,
#if NET6_0_OR_GREATER
        Dictionary<Date, string> namedDates,
        Dictionary<Time, string> namedTimes,
#endif
        Dictionary<DateTime, string> namedDateTimes,
        Dictionary<Guid, string> namedGuids,
        Dictionary<DateTimeOffset, string> namedDateTimeOffsets,
#if NET6_0_OR_GREATER
        IEqualityComparer<Date> dateComparer,
        IEqualityComparer<Time> timeComparer,
#endif
        IEqualityComparer<DateTime> dateTimeComparer,
        IEqualityComparer<Guid> guidComparer,
        IEqualityComparer<DateTimeOffset> dateTimeOffsetComparer)
    {
#if NET6_0_OR_GREATER
        this.namedDates = namedDates;
        this.namedTimes = namedTimes;
#endif
        this.namedDateTimes = namedDateTimes;
        this.namedGuids = namedGuids;
        this.namedDateTimeOffsets = namedDateTimeOffsets;
        this.dateCounting = dateCounting;

#if NET6_0_OR_GREATER
        dateCache = new(dateComparer);
        timeCache = new(timeComparer);
#endif
        Console.WriteLine("HELLO " + guidComparer);
        dateTimeCache = new(dateTimeComparer);
        guidCache = new(guidComparer);
        dateTimeOffsetCache = new(dateTimeOffsetComparer);
    }

    internal static Counter Start(
        bool dateCounting = true,
#if NET6_0_OR_GREATER
        Dictionary<Date, string>? namedDates = null,
        Dictionary<Time, string>? namedTimes = null,
#endif
        Dictionary<DateTime, string>? namedDateTimes = null,
        Dictionary<Guid, string>? namedGuids = null,
        Dictionary<DateTimeOffset, string>? namedDateTimeOffsets = null,
#if NET6_0_OR_GREATER
        IEqualityComparer<Date>? dateComparer = null,
        IEqualityComparer<Time>? timeComparer = null,
#endif
        IEqualityComparer<DateTime>? dateTimeComparer = null,
        IEqualityComparer<Guid>? guidComparer = null,
        IEqualityComparer<DateTimeOffset>? dateTimeOffsetComparer = null)
    {
        var context = new Counter(
            dateCounting,
#if NET6_0_OR_GREATER
            namedDates ?? [],
            namedTimes ?? [],
#endif
            namedDateTimes ?? [],
            namedGuids ?? [],
            namedDateTimeOffsets ?? [],
#if NET6_0_OR_GREATER
            dateComparer ?? new DateComparer(),
            timeComparer ?? new TimeComparer(),
#endif
            dateTimeComparer ?? new DateTimeComparer(),
            guidComparer ?? new GuidComparer(),
            dateTimeOffsetComparer ?? new DateTimeOffsetComparer());
        local.Value = context;
        return context;
    }

    internal static void Stop() =>
        local.Value = null;
}