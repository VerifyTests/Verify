namespace VerifyTests;

public partial class Counter
{
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
        DateCounting = dateCounting;
    }
}
