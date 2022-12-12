namespace VerifyTests;

public partial class Counter
{
#if NET6_0_OR_GREATER

    ConcurrentDictionary<DateOnly, (int intValue, string stringValue)> dateCache = new();
    static Dictionary<DateOnly, string> namedDates = new();
    int currentDate;

    public static void AddNamed(DateOnly value, string name) =>
        namedDates.Add(value, name);

    public int Next(DateOnly input) =>
        NextValue(input).intValue;

    public string NextString(DateOnly input) =>
        NextValue(input).stringValue;

    (int intValue, string stringValue) NextValue(DateOnly input)
    {
        if (namedDates.TryGetValue(input, out var name))
        {
            return new(0, name);
        }

        return dateCache.GetOrAdd(input, _ =>
        {
            var value = Interlocked.Increment(ref currentDate);
            return (value, $"Date_{value}");
        });
    }
#endif
}