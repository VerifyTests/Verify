namespace VerifyTests;

public partial class Counter
{
    ConcurrentDictionary<object, (int intValue, string stringValue)> idCache = new();
    int currentId;

    public int NextId(object input) =>
        NextValue(input).intValue;

    public string NextIdString(object input) =>
        NextValue(input).stringValue;

    (int intValue, string stringValue) NextValue(object input) =>
        idCache.GetOrAdd(input, _ =>
        {
            var value = Interlocked.Increment(ref currentId);
            return (value, $"Id_{value}");
        });
}