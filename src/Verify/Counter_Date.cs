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

        return dateCache.GetOrAdd(
            input,
            _ => BuildDateValue());
    }

    (int intValue, string stringValue) BuildDateValue()
    {
        var value = Interlocked.Increment(ref currentDate);

        if (dateCounting)
        {
            return (value, $"Date_{value}");
        }

        return (value, "{Scrubbed}");
    }
}
#endif