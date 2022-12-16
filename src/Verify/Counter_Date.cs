#if NET6_0_OR_GREATER
namespace VerifyTests;

public partial class Counter
{
    ConcurrentDictionary<Date, (int intValue, string stringValue)> dateCache = new();
    static Dictionary<Date, string> globalNamedDates = new();
    int currentDate;

    public static void AddNamed(Date value, string name) =>
        globalNamedDates.Add(value, name);

    public int Next(Date input) =>
        NextValue(input).intValue;

    public string NextString(Date input) =>
        NextValue(input).stringValue;

    (int intValue, string stringValue) NextValue(Date input)
    {
        if (globalNamedDates.TryGetValue(input, out var name))
        {
            return new(0, name);
        }

        return dateCache.GetOrAdd(input, _ =>
        {
            var value = Interlocked.Increment(ref currentDate);
            return (value, $"Date_{value}");
        });
    }
}
#endif