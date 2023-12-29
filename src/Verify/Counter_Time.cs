#if NET6_0_OR_GREATER
namespace VerifyTests;

public partial class Counter
{
    ConcurrentDictionary<Time, (int intValue, string stringValue)> timeCache = [];
    static Dictionary<Time, string> globalNamedTimes = [];
    int currentTime;

    internal static void AddNamed(Time time, string name)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        globalNamedTimes.Add(time, name);
    }

    public int Next(Time input) =>
        NextValue(input)
            .intValue;

    public string NextString(Time input) =>
        NextValue(input)
            .stringValue;

    (int intValue, string stringValue) NextValue(Time input)
    {
        if (namedTimes.TryGetValue(input, out var name) ||
            globalNamedTimes.TryGetValue(input, out name))
        {
            return new(0, name);
        }

        return timeCache.GetOrAdd(
            input,
            _ =>
            {
                var value = Interlocked.Increment(ref currentTime);
                return (value, $"Time_{value}");
            });
    }
}
#endif