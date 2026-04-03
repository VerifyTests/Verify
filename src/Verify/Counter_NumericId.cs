namespace VerifyTests;

public partial class Counter
{
    Dictionary<string, Dictionary<long, (int intValue, string stringValue)>> numericIdCache = [];
    Dictionary<string, int> numericIdCounters = [];

    public int NextNumericId(string entityName, long input) =>
        NextNumericIdValue(entityName, input)
            .intValue;

    public string NextNumericIdString(string entityName, long input) =>
        NextNumericIdValue(entityName, input)
            .stringValue;

    (int intValue, string stringValue) NextNumericIdValue(string entityName, long input)
    {
        if (!numericIdCache.TryGetValue(entityName, out var cache))
        {
            cache = [];
            numericIdCache[entityName] = cache;
        }

        return cache.GetOrAdd(
            input,
            _ => BuildNumericIdValue(entityName));
    }

    (int intValue, string stringValue) BuildNumericIdValue(string entityName)
    {
        numericIdCounters.TryGetValue(entityName, out var current);
        current++;
        numericIdCounters[entityName] = current;
        return (current, $"{entityName}_{current}");
    }
}
