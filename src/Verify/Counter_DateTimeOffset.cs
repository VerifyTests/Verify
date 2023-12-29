namespace VerifyTests;

public partial class Counter
{
    ConcurrentDictionary<DateTimeOffset, (int intValue, string stringValue)> dateTimeOffsetCache = new(new DateTimeOffsetComparer());
    static Dictionary<DateTimeOffset, string> globalNamedDateTimeOffsets = [];

    class DateTimeOffsetComparer :
        IEqualityComparer<DateTimeOffset>
    {
        public bool Equals(DateTimeOffset x, DateTimeOffset y) =>
            x == y && x.Offset == y.Offset;

        public int GetHashCode(DateTimeOffset obj) =>
            obj.GetHashCode() + (int) obj.Offset.TotalMinutes;
    }

    int currentDateTimeOffset;

    internal static void AddNamed(DateTimeOffset value, string name)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        globalNamedDateTimeOffsets.Add(value, name);
    }

    public int Next(DateTimeOffset input) =>
        NextValue(input)
            .intValue;

    public string NextString(DateTimeOffset input) =>
        NextValue(input)
            .stringValue;

    (int intValue, string stringValue) NextValue(DateTimeOffset input)
    {
        if (namedDateTimeOffsets.TryGetValue(input, out var name) ||
            globalNamedDateTimeOffsets.TryGetValue(input, out name))
        {
            return new(0, name);
        }

        return dateTimeOffsetCache.GetOrAdd(
            input,
            _ =>
            {
                var value = Interlocked.Increment(ref currentDateTimeOffset);

                if (dateCounting)
                {
                    return (value, $"DateTimeOffset_{value}");
                }

                return (value, "{Scrubbed}");
            });
    }
}