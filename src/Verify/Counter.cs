namespace VerifyTests;

public partial class Counter
{
#if NET6_0_OR_GREATER
    Dictionary<DateOnly, string> namedDates;
    Dictionary<TimeOnly, string> namedTimes;
#endif
    Dictionary<DateTime, string> namedDateTimes;
    Dictionary<Guid, string> namedGuids;
    Dictionary<DateTimeOffset, string> namedDateTimeOffsets;
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
    }

    internal static Counter Start(
#if NET6_0_OR_GREATER
        Dictionary<Date, string>? namedDates = null,
        Dictionary<Time, string>? namedTimes = null,
#endif
        Dictionary<DateTime, string>? namedDateTimes = null,
        Dictionary<Guid, string>? namedGuids = null,
        Dictionary<DateTimeOffset, string>? namedDateTimeOffsets = null)
    {
        var context = new Counter(
#if NET6_0_OR_GREATER
            namedDates ?? new(),
            namedTimes ?? new(),
#endif
            namedDateTimes ?? new(),
            namedGuids ?? new(),
            namedDateTimeOffsets ?? new());
        local.Value = context;
        return context;
    }

    internal static void Stop() =>
        local.Value = null;
}

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

public partial class SettingsTask
{
#if NET6_0_OR_GREATER

    public SettingsTask AddNamedDate(Date value, string name)
    {
        CurrentSettings.AddNamedDate(value, name);
        return this;
    }

    public SettingsTask AddNamedTime(Time value, string name)
    {
        CurrentSettings.AddNamedTime(value, name);
        return this;
    }

#endif

    public SettingsTask AddNamedDateTime(DateTime value, string name)
    {
        CurrentSettings.AddNamedDateTime(value, name);
        return this;
    }

    public SettingsTask AddNamedDateTimeOffset(DateTimeOffset value, string name)
    {
        CurrentSettings.AddNamedDateTimeOffset(value, name);
        return this;
    }

    public SettingsTask AddNamedGuid(Guid value, string name)
    {
        CurrentSettings.AddNamedGuid(value, name);
        return this;
    }
}

public partial class VerifierSettings
{
#if NET6_0_OR_GREATER

    public static void AddNamedDate(Date value, string name) =>
        Counter.AddNamed(value, name);

    public static void AddNamedTime(Time value, string name) =>
        Counter.AddNamed(value, name);

#endif

    public static void AddNamedDateTime(DateTime value, string name) =>
        Counter.AddNamed(value, name);

    public static void AddNamedGuid(Guid value, string name) =>
        Counter.AddNamed(value, name);

    public static void AddNamedDateTimeOffset(DateTimeOffset value, string name) =>
        Counter.AddNamed(value, name);
}