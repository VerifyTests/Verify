namespace VerifyTests;

public partial class Counter
{
#if NET6_0_OR_GREATER
    Dictionary<Date, string> namedDates;
    Dictionary<Time, string> namedTimes;
#endif
    Dictionary<DateTime, string> namedDateTimes;
    Dictionary<Guid, string> namedGuids;
    Dictionary<long, string> namedNumbers;
    Dictionary<DateTimeOffset, string> namedDateTimeOffsets;
    bool dateCounting;
    static AsyncLocal<Counter?> local = new();

    internal bool TryGetNamed(object value, [NotNullWhen(true)] out string? result)
    {
#if NET6_0_OR_GREATER

        if (value is Date date)
        {
            if (namedDates.TryGetValue(date, out result) ||
                globalNamedDates.TryGetValue(date, out result))
            {
                return true;
            }

            result = null;
            return false;
        }

        if (value is Time time)
        {
            if (namedTimes.TryGetValue(time, out result) ||
                globalNamedTimes.TryGetValue(time, out result))
            {
                return true;
            }

            result = null;
            return false;
        }

#endif

        if (value is Guid guid)
        {
            if (namedGuids.TryGetValue(guid, out result) ||
                globalNamedGuids.TryGetValue(guid, out result))
            {
                return true;
            }

            result = null;
            return false;
        }

        if (value is int intValue)
        {
            if (namedNumbers.TryGetValue(intValue, out result) ||
                globalNamedNumbers.TryGetValue(intValue, out result))
            {
                return true;
            }

            result = null;
            return false;
        }

        if (value is long longValue)
        {
            if (namedNumbers.TryGetValue(longValue, out result) ||
                globalNamedNumbers.TryGetValue(longValue, out result))
            {
                return true;
            }

            result = null;
            return false;
        }

        if (value is DateTime dateTime)
        {
            if (namedDateTimes.TryGetValue(dateTime, out result) ||
                globalNamedDateTimes.TryGetValue(dateTime, out result))
            {
                return true;
            }

            result = null;
            return false;
        }

        if (value is DateTimeOffset dateTimeOffset)
        {
            if (namedDateTimeOffsets.TryGetValue(dateTimeOffset, out result) ||
                globalNamedDateTimeOffsets.TryGetValue(dateTimeOffset, out result))
            {
                return true;
            }

            result = null;
            return false;
        }

        result = null;
        return false;
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

    public Counter(
        bool dateCounting,
#if NET6_0_OR_GREATER
        Dictionary<Date, string> namedDates,
        Dictionary<Time, string> namedTimes,
#endif
        Dictionary<DateTime, string> namedDateTimes,
        Dictionary<Guid, string> namedGuids,
        Dictionary<long, string> namedNumbers,
        Dictionary<DateTimeOffset, string> namedDateTimeOffsets)
    {
#if NET6_0_OR_GREATER
        this.namedDates = namedDates;
        this.namedTimes = namedTimes;
#endif
        this.namedDateTimes = namedDateTimes;
        this.namedGuids = namedGuids;
        this.namedNumbers = namedNumbers;
        this.namedDateTimeOffsets = namedDateTimeOffsets;
        this.dateCounting = dateCounting;
    }

    internal static Counter Start(
        bool dateCounting = true,
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