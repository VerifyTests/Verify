using System.Diagnostics.CodeAnalysis;
using System.Globalization;

partial class SharedScrubber
{
    internal static List<string> dateFormats = new(){"d"};
    internal static List<string> datetimeFormats = new();
    internal static List<string> datetimeOffsetFormats = new();
    bool scrubDateTimes;

    public bool TryConvert(DateTime value, [NotNullWhen(true)] out string? result)
    {
        if (!scrubDateTimes)
        {
            result = null;
            return false;
        }

        result = Convert(value);
        return true;
    }

#if NET6_0_OR_GREATER

    public bool TryParseConvertDate(string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            foreach (var format in dateFormats)
            {
                if (DateOnly.TryParseExact(value, format, null, DateTimeStyles.None, out var date))
                {
                    result = Convert(date);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    public bool TryConvert(DateOnly value, [NotNullWhen(true)] out string? result)
    {
        if (!scrubDateTimes)
        {
            result = null;
            return false;
        }

        result = Convert(value);
        return true;
    }

    static string Convert(DateOnly date)
    {
        if (date == DateOnly.MaxValue)
        {
            return "Date_MaxValue";
        }

        if (date == DateOnly.MinValue)
        {
            return "Date_MinValue";
        }

        var next = CounterContext.Current.Next(date);
        return $"Date_{next}";
    }

#endif

    public bool TryConvert(DateTimeOffset value, [NotNullWhen(true)] out string? result)
    {
        if (!scrubDateTimes)
        {
            result = null;
            return false;
        }

        result = Convert(value);
        return true;
    }
  
    static string Convert(DateTime date)
    {
        if (date == DateTime.MaxValue)
        {
            return "DateTime_MaxValue";
        }

        if (date == DateTime.MinValue)
        {
            return "DateTime_MinValue";
        }
        
        var next = CounterContext.Current.Next(date);
        return $"DateTime_{next}";
    }

    static string Convert(DateTimeOffset date)
    {
        if (date == DateTimeOffset.MaxValue)
        {
            return "DateTimeOffset_MaxValue";
        }

        if (date == DateTimeOffset.MinValue)
        {
            return "DateTimeOffset_MinValue";
        }

        var next = CounterContext.Current.Next(date);
        return $"DateTimeOffset_{next}";
    }

    public bool TryParseConvertDateTime(string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            if (DateTime.TryParseExact(value, settings.DateFormatString, null, DateTimeStyles.None, out var dateTime))
            {
                result = Convert(dateTime);
                return true;
            }

            foreach (var format in datetimeFormats)
            {
                if (DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out dateTime))
                {
                    result = Convert(dateTime);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    public bool TryParseConvertDateTimeOffset(string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            if (DateTimeOffset.TryParseExact(value, settings.DateFormatString, null, DateTimeStyles.None, out var dateTimeOffset))
            {
                result = Convert(dateTimeOffset);
                return true;
            }

            foreach (var format in datetimeOffsetFormats)
            {
                if (DateTimeOffset.TryParseExact(value, format, null, DateTimeStyles.None, out dateTimeOffset))
                {
                    result = Convert(dateTimeOffset);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }
}