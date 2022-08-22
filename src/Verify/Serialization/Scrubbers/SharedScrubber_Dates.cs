﻿partial class SerializationSettings
{
    internal static List<string> dateFormats = new() {"d"};
    internal static List<string> timeFormats = new() {"h:mm tt"};
    internal static List<string> datetimeFormats = new();
    internal static List<string> datetimeOffsetFormats = new();

    internal bool TryConvert(Counter counter, DateTime value, [NotNullWhen(true)] out string? result)
    {
        if (!scrubDateTimes)
        {
            result = null;
            return false;
        }

        result = Convert(counter, value);
        return true;
    }

#if NET6_0_OR_GREATER
    bool TryParseConvertDate(Counter counter, string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            foreach (var format in dateFormats)
            {
                if (DateOnly.TryParseExact(value, format, null, DateTimeStyles.None, out var date))
                {
                    result = Convert(counter, date);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    internal bool TryConvert(Counter counter, DateOnly value, [NotNullWhen(true)] out string? result)
    {
        if (!scrubDateTimes)
        {
            result = null;
            return false;
        }

        result = Convert(counter, value);
        return true;
    }

    static string Convert(Counter counter, DateOnly date)
    {
        if (date == DateOnly.MaxValue)
        {
            return "Date_MaxValue";
        }

        if (date == DateOnly.MinValue)
        {
            return "Date_MinValue";
        }

        return counter.NextString(date);
    }

    bool TryParseConvertTime(Counter counter, string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            foreach (var format in timeFormats)
            {
                if (TimeOnly.TryParseExact(value, format, null, DateTimeStyles.None, out var time))
                {
                    result = Convert(counter, time);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    internal bool TryConvert(Counter counter, TimeOnly value, [NotNullWhen(true)] out string? result)
    {
        if (!scrubDateTimes)
        {
            result = null;
            return false;
        }

        result = Convert(counter, value);
        return true;
    }

    static string Convert(Counter counter, TimeOnly time)
    {
        if (time == TimeOnly.MaxValue)
        {
            return "Date_MaxValue";
        }

        if (time == TimeOnly.MinValue)
        {
            return "Date_MinValue";
        }

        return counter.NextString(time);
    }

#endif

    internal bool TryConvert(Counter counter, DateTimeOffset value, [NotNullWhen(true)] out string? result)
    {
        if (!scrubDateTimes)
        {
            result = null;
            return false;
        }

        result = Convert(counter, value);
        return true;
    }

    static string Convert(Counter counter, DateTime date)
    {
        if (date.Date == DateTime.MaxValue.Date)
        {
            return "Date_MaxValue";
        }

        if (date.Date == DateTime.MinValue.Date)
        {
            return "Date_MinValue";
        }

        return counter.NextString(date);
    }

    static string Convert(Counter counter, DateTimeOffset date)
    {
        if (date.Date == DateTime.MaxValue.Date)
        {
            return "Date_MaxValue";
        }

        if (date.Date == DateTime.MinValue.Date)
        {
            return "Date_MinValue";
        }

        return counter.NextString(date);
    }

    internal bool TryParseConvertDateTime(Counter counter, string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", null, DateTimeStyles.None, out var dateTime))
            {
                result = Convert(counter, dateTime);
                return true;
            }

            foreach (var format in datetimeFormats)
            {
                if (DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out dateTime))
                {
                    result = Convert(counter, dateTime);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    internal bool TryParseConvertDateTimeOffset(Counter counter, string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            if (DateTimeOffset.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", null, DateTimeStyles.None, out var dateTimeOffset))
            {
                result = Convert(counter, dateTimeOffset);
                return true;
            }

            foreach (var format in datetimeOffsetFormats)
            {
                if (DateTimeOffset.TryParseExact(value, format, null, DateTimeStyles.None, out  dateTimeOffset))
                {
                    result = Convert(counter, dateTimeOffset);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }
}