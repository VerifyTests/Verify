﻿namespace VerifyTests;

public class Counter
{
    static AsyncLocal<Counter?> local = new();

    ConcurrentDictionary<object, (int intValue, string stringValue)> idCache = new();
    int currentId;

    public int NextId(object input) =>
        NextValue(input).intValue;

    public string NextIdString(object input) =>
        NextValue(input).stringValue;

    (int intValue, string stringValue) NextValue(object input) =>
        idCache.GetOrAdd(input, _ =>
        {
            var value = Interlocked.Increment(ref currentId);
            return (value, $"Id_{value}");
        });

    ConcurrentDictionary<Guid, (int intValue, string stringValue)> guidCache = new();
    int currentGuid;

    public int Next(Guid input) =>
        NextValue(input).intValue;

    public string NextString(Guid input) =>
        NextValue(input).stringValue;

    (int intValue, string stringValue) NextValue(Guid input) =>
        guidCache.GetOrAdd(input, _ =>
        {
            var value = Interlocked.Increment(ref currentGuid);
            return (value, $"Guid_{value}");
        });

    ConcurrentDictionary<DateTimeOffset, (int intValue, string stringValue)> dateTimeOffsetCache = new();
    int currentDateTimeOffset;

    public int Next(DateTimeOffset input) =>
        NextValue(input).intValue;

    public string NextString(DateTimeOffset input) =>
        NextValue(input).stringValue;

    (int intValue, string stringValue) NextValue(DateTimeOffset input) =>
        dateTimeOffsetCache.GetOrAdd(input, _ =>
        {
            var value = Interlocked.Increment(ref currentDateTimeOffset);
            return (value, $"DateTimeOffset_{value}");
        });

    ConcurrentDictionary<DateTime, (int intValue, string stringValue)> dateTimeCache = new();
    int currentDateTime;

    public int Next(DateTime input) =>
        NextValue(input).intValue;

    public string NextString(DateTime input) =>
        NextValue(input).stringValue;

    (int intValue, string stringValue) NextValue(DateTime input) =>
        dateTimeCache.GetOrAdd(input, _ =>
        {
            var value = Interlocked.Increment(ref currentDateTime);
            return (value, $"DateTime_{value}");
        });

#if NET6_0_OR_GREATER
    ConcurrentDictionary<DateOnly, (int intValue, string stringValue)> dateCache = new();
    int currentDate;

    public int Next(DateOnly input) =>
        NextValue(input).intValue;

    public string NextString(DateOnly input) =>
        NextValue(input).stringValue;

    (int intValue, string stringValue) NextValue(DateOnly input) =>
        dateCache.GetOrAdd(input, _ =>
        {
            var value = Interlocked.Increment(ref currentDate);
            return (value, $"Date_{value}");
        });

#endif

    public static Counter Current
    {
        get
        {
            var context = local.Value;
            if (context is null)
            {
                throw new("No current context");
            }

            return context;
        }
    }

    internal static Counter Start()
    {
        var context = new Counter();
        local.Value = context;
        return context;
    }

    internal static void Stop() =>
        local.Value = null;
}