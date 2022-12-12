namespace VerifyTests;

public partial class Counter
{
#if NET6_0_OR_GREATER

    ConcurrentDictionary<DateOnly, (int intValue, string stringValue)> dateCache = new();
    int currentDate;

    public int Next(DateOnly input) =>
        NextValue(input).intValue;

    public string NextString(DateOnly input) =>
        NextValue(input).stringValue;

    (int intValue, string stringValue) NextValue(DateOnly input) =>
        dateCache.GetOrAdd(input, _ =>
        {
            var value = Interlocked.Increment(ref currentDate);
            return (value, $"Date_{value}");
        });
#endif
}