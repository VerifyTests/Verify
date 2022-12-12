namespace VerifyTests;

public partial class Counter
{
    ConcurrentDictionary<Guid, (int intValue, string stringValue)> guidCache = new();
    static Dictionary<Guid, string> namedGuids = new();
    int currentGuid;

    public int Next(Guid input) =>
        NextValue(input).intValue;

    public static void AddNamedGuid(Guid guid, string name) =>
        namedGuids.Add(guid, name);

    public string NextString(Guid input) =>
        NextValue(input).stringValue;

    (int intValue, string stringValue) NextValue(Guid input)
    {
        if (namedGuids.TryGetValue(input, out var value))
        {
            return new(0, value);
        }

        return guidCache.GetOrAdd(input, _ =>
        {
            var value = Interlocked.Increment(ref currentGuid);
            return (value, $"Guid_{value}");
        });
    }
}