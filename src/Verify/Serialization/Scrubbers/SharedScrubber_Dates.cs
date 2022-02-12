namespace VerifyTests;

public partial class SerializationSettings
{
    internal static List<string> dateFormats = new() {"d"};
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

        var next = counter.Next(date);
        return $"Date_{next}";
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

        var next = counter.Next(date);
        return $"DateTime_{next}";
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

        var next = counter.Next(date);
        return $"DateTimeOffset_{next}";
    }

    internal bool TryParseConvertDateTime(Counter counter, string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            if (DateTime.TryParseExact(value, serializersettings.DateFormatString, null, DateTimeStyles.None, out var dateTime))
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
            if (DateTimeOffset.TryParseExact(value, serializersettings.DateFormatString, null, DateTimeStyles.None, out var dateTimeOffset))
            {
                result = Convert(counter, dateTimeOffset);
                return true;
            }

            foreach (var format in datetimeOffsetFormats)
            {
                if (DateTimeOffset.TryParseExact(value, format, null, DateTimeStyles.None, out dateTimeOffset))
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