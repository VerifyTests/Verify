namespace VerifyTests;

public partial class Counter
{
    Dictionary<Guid, (int intValue, string stringValue)> guidCache = [];
    static Dictionary<Guid, string> globalNamedGuids = [];
    int currentGuid;

    public int Next(Guid input) =>
        NextValue(input)
            .intValue;

    internal static void AddNamed(Guid value, string name)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        globalNamedGuids.Add(value, name);
    }

    public string NextString(Guid input) =>
        NextValue(input)
            .stringValue;

    (int intValue, string stringValue) NextValue(Guid input)
    {
        if (namedGuids.TryGetValue(input, out var name) ||
            globalNamedGuids.TryGetValue(input, out name))
        {
            return new(0, name);
        }

        lock (cacheLock)
        {
            return guidCache.GetOrAdd(
                input,
                _ => BuildGuidValue());
        }
    }

    // Called under cacheLock
    (int intValue, string stringValue) BuildGuidValue()
    {
        var value = ++currentGuid;
        return (value, $"Guid_{value}");
    }
}