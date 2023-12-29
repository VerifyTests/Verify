namespace VerifyTests;

public partial class Counter
{
    ConcurrentDictionary<DateTime, (int intValue, string stringValue)> dateTimeCache = new(new DateTimeComparer());
    static Dictionary<DateTime, string> globalNamedDateTimes = [];

    class DateTimeComparer : IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime x, DateTime y) =>
            x == y && x.Kind == y.Kind;

        public int GetHashCode(DateTime obj) =>
            obj.GetHashCode() + (int) obj.Kind;
    }

    int currentDateTime;

    internal static void AddNamed(DateTime value, string name)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        globalNamedDateTimes.Add(value, name);
    }

    public int Next(DateTime input) =>
        NextValue(input)
            .intValue;

    public string NextString(DateTime input) =>
        NextValue(input)
            .stringValue;

    (int intValue, string stringValue) NextValue(DateTime input)
    {
        if (namedDateTimes.TryGetValue(input, out var name) ||
            globalNamedDateTimes.TryGetValue(input, out name))
        {
            return new(0, name);
        }

        return dateTimeCache.GetOrAdd(
            input,
            _ =>
            {
                var value = Interlocked.Increment(ref currentDateTime);

                if (dateCounting)
                {
                    return (value, $"DateTime_{value}");
                }

                return (value, "{Scrubbed}");
            });
    }
}