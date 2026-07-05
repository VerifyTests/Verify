namespace VerifyTests;

public partial class Counter
{
    Dictionary<string, Dictionary<string, (int intValue, string stringValue)>> numericIdCache = [];
    Dictionary<string, int> numericIdCounters = [];

    public int NextNumericId(string entityName, long input) =>
        NextNumericIdValue(entityName, input.ToString(CultureInfo.InvariantCulture))
            .intValue;

    public string NextNumericIdString(string entityName, long input) =>
        NextNumericIdValue(entityName, input.ToString(CultureInfo.InvariantCulture))
            .stringValue;

    // Keys on the invariant string form of the value rather than converting to
    // long, so ulong/decimal/double ids outside Int64 range (and NaN) don't throw,
    // and distinct fractional values are not collapsed by rounding.
    internal string NextNumericIdString(string entityName, object value) =>
        NextNumericIdValue(entityName, ToKey(value))
            .stringValue;

    static string ToKey(object value)
    {
        if (value is IFormattable formattable)
        {
            return formattable.ToString(null, CultureInfo.InvariantCulture);
        }

        return value.ToString()!;
    }

    (int intValue, string stringValue) NextNumericIdValue(string entityName, string input)
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
