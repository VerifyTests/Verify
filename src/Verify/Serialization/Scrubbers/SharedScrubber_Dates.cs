namespace VerifyTests;

partial class Counter
{
#if NET6_0_OR_GREATER
    internal static List<string> dateFormats = ["d"];
    internal static List<string> timeFormats = ["h:mm tt"];
#endif
    internal static List<string> dateTimeFormats = [];
    internal static List<string> dateTimeOffsetFormats = [];

    public bool TryConvert(DateTime value, [NotNullWhen(true)] out string? result)
    {
        if (!ScrubDateTimes)
        {
            result = null;
            return false;
        }

        result = Convert(value);
        return true;
    }

#if NET6_0_OR_GREATER
    public bool TryConvertDate(CharSpan value, [NotNullWhen(true)] out string? result)
    {
        if (ScrubDateTimes)
        {
            foreach (var format in dateFormats)
            {
                if (Date.TryParseExact(value, format, null, DateTimeStyles.None, out var date))
                {
                    result = Convert(date);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    public bool TryConvert(Date value, [NotNullWhen(true)] out string? result)
    {
        if (!ScrubDateTimes)
        {
            result = null;
            return false;
        }

        result = Convert(value);
        return true;
    }

    internal string Convert(Date date)
    {
        if (date == Date.MaxValue)
        {
            return "Date_MaxValue";
        }

        if (date == Date.MinValue)
        {
            return "Date_MinValue";
        }

        return NextString(date);
    }

    public bool TryConvertTime(CharSpan value, [NotNullWhen(true)] out string? result)
    {
        if (ScrubDateTimes)
        {
            foreach (var format in timeFormats)
            {
                if (Time.TryParseExact(value, format, null, DateTimeStyles.None, out var time))
                {
                    result = Convert(time);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    public bool TryConvert(Time value, [NotNullWhen(true)] out string? result)
    {
        if (ScrubDateTimes)
        {
            result = Convert(value);
            return true;
        }

        result = null;
        return false;
    }

    string Convert(Time time)
    {
        if (time == Time.MaxValue)
        {
            return "Time_MaxValue";
        }

        if (time == Time.MinValue)
        {
            return "Time_MinValue";
        }

        return NextString(time);
    }

#endif

    public bool TryConvert(DateTimeOffset value, [NotNullWhen(true)] out string? result)
    {
        if (!ScrubDateTimes)
        {
            result = null;
            return false;
        }

        result = Convert(value);
        return true;
    }

    internal string Convert(DateTime date)
    {
        if (date.Date == DateTime.MaxValue.Date)
        {
            return "Date_MaxValue";
        }

        if (date.Date == DateTime.MinValue.Date)
        {
            return "Date_MinValue";
        }

        return NextString(date);
    }

    internal string Convert(DateTimeOffset date)
    {
        if (date.Date == DateTime.MaxValue.Date)
        {
            return "Date_MaxValue";
        }

        if (date.Date == DateTime.MinValue.Date)
        {
            return "Date_MinValue";
        }

        return NextString(date);
    }

    public bool TryConvertDateTime(CharSpan value, [NotNullWhen(true)] out string? result)
    {
        if (ScrubDateTimes)
        {
            if (TryParseDateTime(value, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", out var date))
            {
                result = Convert(date);
                return true;
            }

            foreach (var format in dateTimeFormats)
            {
                if (TryParseDateTime(value, format, out date))
                {
                    result = Convert(date);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    static bool TryParseDateTime(CharSpan value, string format, out DateTime dateTime) =>
        DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out dateTime);

    public bool TryConvertDateTimeOffset(CharSpan value, [NotNullWhen(true)] out string? result)
    {
        if (ScrubDateTimes)
        {
            if (TryParseDateTimeOffset(value, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", out var date))
            {
                result = Convert(date);
                return true;
            }

            foreach (var format in dateTimeOffsetFormats)
            {
                if (TryParseDateTimeOffset(value, format, out date))
                {
                    result = Convert(date);
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