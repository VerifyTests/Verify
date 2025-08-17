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
    public bool DateCounting { get; }
    public bool ScrubDateTimes { get; }
    public bool ScrubGuids { get; }
    static AsyncLocal<Counter?> local = new();

    internal bool TryGetNamed(object value, [NotNullWhen(true)] out string? result)
    {
        switch (value)
        {
#if NET6_0_OR_GREATER
            case Date date
                when namedDates.TryGetValue(date, out result) ||
                     globalNamedDates.TryGetValue(date, out result):
                return true;
            case Date:
                result = null;
                return false;
            case Time time
                when namedTimes.TryGetValue(time, out result) ||
                     globalNamedTimes.TryGetValue(time, out result):
                return true;
            case Time:
                result = null;
                return false;
#endif
            case Guid guid
                when namedGuids.TryGetValue(guid, out result) ||
                     globalNamedGuids.TryGetValue(guid, out result):
                return true;
            case Guid:
                result = null;
                return false;
            case DateTime dateTime
                when namedDateTimes.TryGetValue(dateTime, out result) ||
                     globalNamedDateTimes.TryGetValue(dateTime, out result):
                return true;
            case DateTime:
                result = null;
                return false;
            case DateTimeOffset dateTimeOffset
                when namedDateTimeOffsets.TryGetValue(dateTimeOffset, out result) ||
                     globalNamedDateTimeOffsets.TryGetValue(dateTimeOffset, out result):
                return true;
            case DateTimeOffset:
                result = null;
                return false;
            default:
                result = null;
                return false;
        }
    }

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

    public static Counter? CurrentOrNull => local.Value;
    [Obsolete("Use overload with scrubDateTimes and scrubGuids parameters")]
    public Counter(
        bool dateCounting,
#if NET6_0_OR_GREATER
        Dictionary<Date, string> namedDates,
        Dictionary<Time, string> namedTimes,
#endif
        Dictionary<DateTime, string> namedDateTimes,
        Dictionary<Guid, string> namedGuids,
        Dictionary<DateTimeOffset, string> namedDateTimeOffsets)
    {
#if NET6_0_OR_GREATER
        this.namedDates = namedDates;
        this.namedTimes = namedTimes;
#endif
        this.namedDateTimes = namedDateTimes;
        this.namedGuids = namedGuids;
        this.namedDateTimeOffsets = namedDateTimeOffsets;
        this.DateCounting = dateCounting;
    }
    public Counter(
        bool dateCounting,
        bool scrubDateTimes,
        bool scrubGuids,
#if NET6_0_OR_GREATER
        Dictionary<Date, string> namedDates,
        Dictionary<Time, string> namedTimes,
#endif
        Dictionary<DateTime, string> namedDateTimes,
        Dictionary<Guid, string> namedGuids,
        Dictionary<DateTimeOffset, string> namedDateTimeOffsets)
    {
#if NET6_0_OR_GREATER
        this.namedDates = namedDates;
        this.namedTimes = namedTimes;
#endif
        this.namedDateTimes = namedDateTimes;
        this.namedGuids = namedGuids;
        this.namedDateTimeOffsets = namedDateTimeOffsets;
        this.DateCounting = dateCounting;
        this.ScrubDateTimes = scrubDateTimes;
        this.ScrubGuids = scrubGuids;
    }

    internal static Counter Start(
        bool dateCounting = true,
        bool scrubDateTimes = true,
        bool scrubGuids = true,
#if NET6_0_OR_GREATER
        Dictionary<Date, string>? namedDates = null,
        Dictionary<Time, string>? namedTimes = null,
#endif
        Dictionary<DateTime, string>? namedDateTimes = null,
        Dictionary<Guid, string>? namedGuids = null,
        Dictionary<DateTimeOffset, string>? namedDateTimeOffsets = null)
    {
        var context = new Counter(
            dateCounting,
            scrubDateTimes,
            scrubGuids,
#if NET6_0_OR_GREATER
            namedDates ?? [],
            namedTimes ?? [],
#endif
            namedDateTimes ?? [],
            namedGuids ?? [],
            namedDateTimeOffsets ?? []);
        local.Value = context;
        return context;
    }

    internal static void Stop() =>
        local.Value = null;
}