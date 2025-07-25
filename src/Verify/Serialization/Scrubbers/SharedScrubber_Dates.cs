﻿partial class SerializationSettings
{
#if NET6_0_OR_GREATER
    internal static List<string> dateFormats = ["d"];
    internal static List<string> timeFormats = ["h:mm tt"];
#endif
    internal static List<string> dateTimeFormats = [];
    internal static List<string> dateTimeOffsetFormats = [];

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
    internal bool TryParseConvertDate(Counter counter, CharSpan value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            foreach (var format in dateFormats)
            {
                if (Date.TryParseExact(value, format, null, DateTimeStyles.None, out var date))
                {
                    result = Convert(counter, date);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    internal bool TryConvert(Counter counter, Date value, [NotNullWhen(true)] out string? result)
    {
        if (!scrubDateTimes)
        {
            result = null;
            return false;
        }

        result = Convert(counter, value);
        return true;
    }

    internal static string Convert(Counter counter, Date date)
    {
        if (date == Date.MaxValue)
        {
            return "Date_MaxValue";
        }

        if (date == Date.MinValue)
        {
            return "Date_MinValue";
        }

        return counter.NextString(date);
    }

    internal bool TryParseConvertTime(Counter counter, CharSpan value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            foreach (var format in timeFormats)
            {
                if (Time.TryParseExact(value, format, null, DateTimeStyles.None, out var time))
                {
                    result = Convert(counter, time);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    internal bool TryConvert(Counter counter, Time value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            result = Convert(counter, value);
            return true;
        }

        result = null;
        return false;
    }

    static string Convert(Counter counter, Time time)
    {
        if (time == Time.MaxValue)
        {
            return "Time_MaxValue";
        }

        if (time == Time.MinValue)
        {
            return "Time_MinValue";
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

    internal static string Convert(Counter counter, DateTime date)
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

    internal static string Convert(Counter counter, DateTimeOffset date)
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

    internal bool TryParseConvertDateTime(Counter counter, CharSpan value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            if (TryParseDateTime(value, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", out var dateTime))
            {
                result = Convert(counter, dateTime);
                return true;
            }

            foreach (var format in dateTimeFormats)
            {
                if (TryParseDateTime(value, format, out dateTime))
                {
                    result = Convert(counter, dateTime);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    static bool TryParseDateTime(CharSpan value, string format, out DateTime dateTime) =>
        DateTimePolyfill.TryParseExact(value, format, null, DateTimeStyles.None, out dateTime);

    internal bool TryParseConvertDateTimeOffset(Counter counter, CharSpan value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            if (TryParseDateTimeOffset(value, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", out var dateTimeOffset))
            {
                result = Convert(counter, dateTimeOffset);
                return true;
            }

            foreach (var format in dateTimeOffsetFormats)
            {
                if (TryParseDateTimeOffset(value, format, out dateTimeOffset))
                {
                    result = Convert(counter, dateTimeOffset);
                    return true;
                }
            }
        }

        result = null;
        return false;

    }

    static bool TryParseDateTimeOffset(CharSpan value, string format, out DateTimeOffset dateTimeOffset) =>
        DateTimeOffset.TryParseExact(value, format, null, DateTimeStyles.None, out dateTimeOffset);
}