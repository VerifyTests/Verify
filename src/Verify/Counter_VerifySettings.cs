namespace VerifyTests;

public partial class VerifySettings
{
#if NET6_0_OR_GREATER

    internal Dictionary<Date, string> namedDates = new();

    public void AddNamedDate(Date value, string name) =>
        namedDates.Add(value, name);

    internal Dictionary<Time, string> namedTimes = new();

    public void AddNamedTime(Time value, string name) =>
        namedTimes.Add(value, name);

#endif

    internal Dictionary<DateTime, string> namedDateTimes = new();

    public void AddNamedDateTime(DateTime value, string name) =>
        namedDateTimes.Add(value, name);

    internal Dictionary<Guid, string> namedGuids = new();

    public void AddNamedGuid(Guid value, string name) =>
        namedGuids.Add(value, name);

    internal Dictionary<DateTimeOffset, string> namedDateTimeOffsets = new();

    public void AddNamedDateTimeOffset(DateTimeOffset value, string name) =>
        namedDateTimeOffsets.Add(value, name);
}