using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

class SharedScrubber
{
    internal static List<string> datetimeFormats = new List<string>();
    internal static List<string> datetimeOffsetFormats = new List<string>();
    bool scrubGuids;
    bool scrubDateTimes;
    static Func<Guid, int> intOrNextGuid = null!;
    static Func<DateTime, int> intOrNextDateTime = null!;
    static Func<DateTimeOffset, int> intOrNextDateTimeOffset = null!;

    public static void SetIntOrNext(
        Func<Guid, int> guid,
        Func<DateTime, int> dateTime,
        Func<DateTimeOffset, int> dateTimeOffset
    )
    {
        intOrNextGuid = guid;
        intOrNextDateTime = dateTime;
        intOrNextDateTimeOffset = dateTimeOffset;
    }

    public SharedScrubber(bool scrubGuids, bool scrubDateTimes)
    {
        this.scrubGuids = scrubGuids;
        this.scrubDateTimes = scrubDateTimes;
    }

    public string? GetValue(Guid? value)
    {
        if (value == null)
        {
            return null;
        }

        var next = intOrNextGuid(value.Value);
        return $"Guid_{next}";
    }

    public bool TryGetGuid(object? value, [NotNullWhen(true)] out string? result)
    {
        if (value == null)
        {
            result = null;
            return false;
        }

        var guid = (Guid) value;

        if (!scrubGuids)
        {
            result = value.ToString();
            return true;
        }

        result = Convert(guid);
        return true;
    }

    static string Convert(Guid guid)
    {
        var next = intOrNextGuid(guid);
        return $"Guid_{next}";
    }

    static string Convert(DateTime dateTime)
    {
        var next = intOrNextDateTime(dateTime);
        return $"DateTime_{next}";
    }

    static string Convert(DateTimeOffset dateTime)
    {
        var next = intOrNextDateTimeOffset(dateTime);
        return $"DateTimeOffset_{next}";
    }

    public bool TryConvertString(string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubGuids)
        {
            if (Guid.TryParse(value, out var guid))
            {
                result = Convert(guid);
                return true;
            }
        }

        if (scrubDateTimes)
        {
            foreach (var format in datetimeOffsetFormats)
            {
                if (DateTimeOffset.TryParseExact(value, format, null, DateTimeStyles.None, out var dateTimeOffset))
                {
                    result = Convert(dateTimeOffset);
                    return true;
                }
            }

            foreach (var format in datetimeFormats)
            {
                if (DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out var dateTime))
                {
                    result = Convert(dateTime);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }
}