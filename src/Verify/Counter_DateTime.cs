namespace VerifyTests;

public partial class Counter
{
    ConcurrentDictionary<DateTime, (int intValue, string stringValue)> dateTimeCache = new(new DateTimeComparer());
    static Dictionary<DateTime, string> namedDateTimes = new();

    class DateTimeComparer : IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime x, DateTime y) =>
            x == y && x.Kind == y.Kind;

        public int GetHashCode(DateTime obj) =>
            obj.GetHashCode() + (int) obj.Kind;
    }

    int currentDateTime;

    public static void AddNamed(DateTime value, string name) =>
        namedDateTimes.Add(value, name);

    public int Next(DateTime input) =>
        NextValue(input).intValue;

    public string NextString(DateTime input) =>
        NextValue(input).stringValue;

    (int intValue, string stringValue) NextValue(DateTime input)
    {
        if (namedDateTimes.TryGetValue(input, out var name))
        {
            return new(0, name);
        }

        return dateTimeCache.GetOrAdd(input, _ =>
        {
            var value = Interlocked.Increment(ref currentDateTime);
            return (value, $"DateTime_{value}");
        });
    }
}