namespace VerifyTests;

public partial class Counter
{
    public static void UseDateTimeOffsetComparer(IEqualityComparer<DateTimeOffset> comparer)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        dateTimeOffsetComparer = comparer;
    }

    static IEqualityComparer<DateTimeOffset> dateTimeOffsetComparer = new DateTimeOffsetComparer();
    Dictionary<DateTimeOffset, (int intValue, string stringValue)> dateTimeOffsetCache = new(dateTimeOffsetComparer);
    static Dictionary<DateTimeOffset, string> globalNamedDateTimeOffsets = [];

    #region DateTimeOffsetComparer
    class DateTimeOffsetComparer :
        IEqualityComparer<DateTimeOffset>
    {
        public bool Equals(DateTimeOffset x, DateTimeOffset y) =>
            x == y && x.Offset == y.Offset;

        public int GetHashCode(DateTimeOffset obj) =>
            obj.GetHashCode() + (int) obj.Offset.TotalMinutes;
    }
    #endregion

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
            _ => BuildDateTimeOffsetValue());
    }

    (int intValue, string stringValue) BuildDateTimeOffsetValue()
    {
        var value = Interlocked.Increment(ref currentDateTimeOffset);

        if (DateCounting)
        {
            return (value, $"DateTimeOffset_{value}");
        }

        return (value, "{Scrubbed}");
    }
}