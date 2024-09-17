namespace VerifyTests;

public partial class VerifySettings
{
#if NET6_0_OR_GREATER

    internal Dictionary<Date, string> namedDates = [];

    public void AddNamedDate(Date value, string name) =>
        namedDates.Add(value, name);

    internal Dictionary<Time, string> namedTimes = [];

    public void AddNamedTime(Time value, string name) =>
        namedTimes.Add(value, name);

    internal IEqualityComparer<Date>? dateComparer;

    public void ReplaceScrubberDateComparer(IEqualityComparer<Date> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        dateComparer = comparer;
    }

    internal IEqualityComparer<Time>? timeComparer;

    public void ReplaceScrubberTimeComparer(IEqualityComparer<Time> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        timeComparer = comparer;
    }

#endif

    internal Dictionary<DateTime, string> namedDateTimes = [];

    public void AddNamedDateTime(DateTime value, string name) =>
        namedDateTimes.Add(value, name);

    internal Dictionary<Guid, string> namedGuids = [];

    public void AddNamedGuid(Guid value, string name) =>
        namedGuids.Add(value, name);

    internal Dictionary<DateTimeOffset, string> namedDateTimeOffsets = [];

    public void AddNamedDateTimeOffset(DateTimeOffset value, string name) =>
        namedDateTimeOffsets.Add(value, name);

    internal IEqualityComparer<DateTime>? dateTimeComparer;

    public void ReplaceScrubberDateTimeComparer(IEqualityComparer<DateTime> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        dateTimeComparer = comparer;
    }

    internal IEqualityComparer<Guid>? guidComparer;

    public void ReplaceScrubberGuidComparer(IEqualityComparer<Guid> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        guidComparer = comparer;
    }

    internal IEqualityComparer<DateTimeOffset>? dateTimeOffsetComparer;

    public void ReplaceScrubberDateTimeOffsetComparer(IEqualityComparer<DateTimeOffset> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        dateTimeOffsetComparer = comparer;
    }
}