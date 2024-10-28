namespace VerifyTests;

public partial class VerifySettings
{
#if NET6_0_OR_GREATER

    internal Dictionary<Date, string> namedDates = [];

    public VerifySettings AddNamedDate(Date value, string name)
    {
        namedDates.Add(value, name);
        return this;
    }

    internal Dictionary<Time, string> namedTimes = [];

    public VerifySettings AddNamedTime(Time value, string name)
    {
        namedTimes.Add(value, name);
        return this;
    }

#endif

    internal Dictionary<DateTime, string> namedDateTimes = [];

    public VerifySettings AddNamedDateTime(DateTime value, string name)
    {
        namedDateTimes.Add(value, name);
        return this;
    }

    internal Dictionary<Guid, string> namedGuids = [];

    public VerifySettings AddNamedGuid(Guid value, string name)
    {
        namedGuids.Add(value, name);
        return this;
    }

    internal Dictionary<DateTimeOffset, string> namedDateTimeOffsets = [];

    public VerifySettings AddNamedDateTimeOffset(DateTimeOffset value, string name)
    {
        namedDateTimeOffsets.Add(value, name);
        return this;
    }
}