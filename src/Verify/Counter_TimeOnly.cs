namespace VerifyTests;

public partial class Counter
{
#if NET6_0_OR_GREATER

    ConcurrentDictionary<TimeOnly, (int intValue, string stringValue)> timeCache = new();
    int currentTime;

    public int Next(TimeOnly input) =>
        NextValue(input).intValue;

    public string NextString(TimeOnly input) =>
        NextValue(input).stringValue;

    (int intValue, string stringValue) NextValue(TimeOnly input) =>
        timeCache.GetOrAdd(input, _ =>
        {
            var value = Interlocked.Increment(ref currentTime);
            return (value, $"Time_{value}");
        });

#endif
}