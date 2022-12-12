namespace VerifyTests;

public partial class Counter
{
    [Obsolete("Id replacement no longer used")]
    ConcurrentDictionary<object, (int intValue, string stringValue)> idCache = new();

    [Obsolete("Id replacement no longer used")]
    int currentId;

    [Obsolete("Id replacement no longer used")]
    public int NextId(object input) =>
        NextValue(input).intValue;

    [Obsolete("Id replacement no longer used")]
    public string NextIdString(object input) =>
        NextValue(input).stringValue;

    [Obsolete("Id replacement no longer used")]
    (int intValue, string stringValue) NextValue(object input) =>
        idCache.GetOrAdd(input, _ =>
        {
            var value = Interlocked.Increment(ref currentId);
            return (value, $"Id_{value}");
        });
}