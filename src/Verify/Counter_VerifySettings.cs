namespace VerifyTests;

public partial class VerifySettings
{
#if NET6_0_OR_GREATER

    internal Dictionary<Date, string>? namedDates;

    public void AddNamedDate(Date value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        namedDates ??= [];
        namedDates.Add(value, name);
    }

    internal Dictionary<Time, string>? namedTimes;

    public void AddNamedTime(Time value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        namedTimes ??= [];
        namedTimes.Add(value, name);
    }

#endif

    internal Dictionary<DateTime, string>? namedDateTimes;

    public void AddNamedDateTime(DateTime value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        namedDateTimes ??= [];
        namedDateTimes.Add(value, name);
    }

    internal Dictionary<Guid, string>? namedGuids;

    public void AddNamedGuid(Guid value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        namedGuids ??= [];
        namedGuids.Add(value, name);
    }

    internal Dictionary<DateTimeOffset, string>? namedDateTimeOffsets;

    public void AddNamedDateTimeOffset(DateTimeOffset value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        namedDateTimeOffsets ??= [];
        namedDateTimeOffsets.Add(value, name);
    }
}