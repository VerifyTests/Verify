namespace VerifyTests;

public partial class Counter
{
    Dictionary<long, (int intValue, string stringValue)> numericIdCache = [];
    int currentNumericId;

    public int NextNumericId(long input) =>
        NextNumericIdValue(input)
            .intValue;

    public string NextNumericIdString(long input) =>
        NextNumericIdValue(input)
            .stringValue;

    (int intValue, string stringValue) NextNumericIdValue(long input) =>
        numericIdCache.GetOrAdd(
            input,
            _ => BuildNumericIdValue());

    (int intValue, string stringValue) BuildNumericIdValue()
    {
        var value = Interlocked.Increment(ref currentNumericId);
        return (value, $"Id_{value}");
    }
}
