#if NET6_0_OR_GREATER
namespace VerifyTests;

public partial class Counter
{
    Dictionary<Date, (int intValue, string stringValue)> dateCache = [];
    static Dictionary<Date, string> globalNamedDates = [];
    int currentDate;

    internal static void AddNamed(Date value, string name)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        globalNamedDates.Add(value, name);
    }

    public int Next(Date input) =>
        NextValue(input)
            .intValue;

    public string NextString(Date input) =>
        NextValue(input)
            .stringValue;

    (int intValue, string stringValue) NextValue(Date input)
    {
        if (namedDates.TryGetValue(input, out var name) ||
            globalNamedDates.TryGetValue(input, out name))
        {
            return new(0, name);
        }

        lock (cacheLock)
        {
            return dateCache.GetOrAdd(
                input,
                _ => BuildDateValue());
        }
    }

    // Called under cacheLock
    (int intValue, string stringValue) BuildDateValue()
    {
        var value = ++currentDate;

        if (DateCounting)
        {
            return (value, $"Date_{value}");
        }

        return (value, "{Scrubbed}");
    }
}
#else
namespace VerifyTests;

public partial class Counter
{
    Dictionary<DateTime, (int intValue, string stringValue)> dateCache = new(dateTimeComparer);
    int currentDate;

    // Called under cacheLock
    (int intValue, string stringValue) BuildDateValue()
    {
        var value = ++currentDate;

        if (DateCounting)
        {
            return (value, $"Date_{value}");
        }

        return (value, "{Scrubbed}");
    }

    internal string ConvertDate(DateTime date)
    {
        if (date.Date == DateTime.MaxValue.Date)
        {
            return "Date_MaxValue";
        }

        if (date.Date == DateTime.MinValue.Date)
        {
            return "Date_MinValue";
        }

        lock (cacheLock)
        {
            return dateCache.GetOrAdd(
                    date,
                    _ => BuildDateValue())
                .stringValue;
        }
    }
}
#endif
