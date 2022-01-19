using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace VerifyTests;

public partial class SerializationSettings
{
    internal static List<string> dateFormats = new() {"d"};
    internal static List<string> datetimeFormats = new();
    internal static List<string> datetimeOffsetFormats = new();

    internal bool TryConvert(DateTime value, [NotNullWhen(true)] out string? result)
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

    bool TryParseConvertDate(string value, [NotNullWhen(true)] out string? result)
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

    internal bool TryConvert(DateOnly value, [NotNullWhen(true)] out string? result)
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

    internal bool TryConvert(DateTimeOffset value, [NotNullWhen(true)] out string? result)
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
        if (date.Date == DateTime.MaxValue.Date)
        {
            return "Date_MaxValue";
        }

        if (date.Date == DateTime.MinValue.Date)
        {
            return "Date_MinValue";
        }

        var next = CounterContext.Current.Next(date);
        return $"DateTime_{next}";
    }

    static string Convert(DateTimeOffset date)
    {
        if (date.Date == DateTime.MaxValue.Date)
        {
            return "Date_MaxValue";
        }

        if (date.Date == DateTime.MinValue.Date)
        {
            return "Date_MinValue";
        }

        var next = CounterContext.Current.Next(date);
        return $"DateTimeOffset_{next}";
    }

    internal bool TryParseConvertDateTime(string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            if (DateTime.TryParseExact(value, serializersettings.DateFormatString, null, DateTimeStyles.None, out var dateTime))
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

    internal bool TryParseConvertDateTimeOffset(string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            if (DateTimeOffset.TryParseExact(value, serializersettings.DateFormatString, null, DateTimeStyles.None, out var dateTimeOffset))
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