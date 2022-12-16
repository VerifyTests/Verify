﻿namespace VerifyTests;

public partial class Counter
{
    ConcurrentDictionary<DateTimeOffset, (int intValue, string stringValue)> dateTimeOffsetCache = new(new DateTimeOffsetComparer());
    static Dictionary<DateTimeOffset, string> globalNamedDateTimeOffsets = new();

    class DateTimeOffsetComparer :
        IEqualityComparer<DateTimeOffset>
    {
        public bool Equals(DateTimeOffset x, DateTimeOffset y) =>
            x == y && x.Offset == y.Offset;

        public int GetHashCode(DateTimeOffset obj) =>
            obj.GetHashCode() + (int) obj.Offset.TotalMinutes;
    }

    int currentDateTimeOffset;

    public static void AddNamed(DateTimeOffset value, string name) =>
        globalNamedDateTimeOffsets.Add(value, name);

    public int Next(DateTimeOffset input) =>
        NextValue(input).intValue;

    public string NextString(DateTimeOffset input) =>
        NextValue(input).stringValue;

    (int intValue, string stringValue) NextValue(DateTimeOffset input)
    {
        if (globalNamedDateTimeOffsets.TryGetValue(input, out var name))
        {
            return new(0, name);
        }

        return dateTimeOffsetCache.GetOrAdd(input, _ =>
        {
            var value = Interlocked.Increment(ref currentDateTimeOffset);
            return (value, $"DateTimeOffset_{value}");
        });
    }
}