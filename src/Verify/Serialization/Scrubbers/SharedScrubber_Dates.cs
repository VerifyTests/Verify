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

        var counterContext = CounterContext.Current;
        result = Convert(counterContext, value);
        return true;
    }

#if NET6_0_OR_GREATER

    bool TryParseConvertDate(string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            var counterContext = CounterContext.Current;
            foreach (var format in dateFormats)
            {
                if (DateOnly.TryParseExact(value, format, null, DateTimeStyles.None, out var date))
                {
                    result = Convert(counterContext, date);
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

        var counterContext = CounterContext.Current;
        result = Convert(counterContext, value);
        return true;
    }

    private static string Convert(CounterContext counterContext, DateOnly date)
    {
        if (date == DateOnly.MaxValue)
        {
            return "Date_MaxValue";
        }

        if (date == DateOnly.MinValue)
        {
            return "Date_MinValue";
        }

        var next = counterContext.Next(date);
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

        var counterContext = CounterContext.Current;
        result = Convert(counterContext, value);
        return true;
    }

    private static string Convert(CounterContext counterContext, DateTime date)
    {
        if (date.Date == DateTime.MaxValue.Date)
        {
            return "Date_MaxValue";
        }

        if (date.Date == DateTime.MinValue.Date)
        {
            return "Date_MinValue";
        }

        var next = counterContext.Next(date);
        return $"DateTime_{next}";
    }

    static string Convert(CounterContext counter, DateTimeOffset date)
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

    internal bool TryParseConvertDateTime(string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubDateTimes)
        {
            var counterContext = CounterContext.Current;
            if (DateTime.TryParseExact(value, serializersettings.DateFormatString, null, DateTimeStyles.None, out var dateTime))
            {
                result = Convert(counterContext, dateTime);
                return true;
            }

            foreach (var format in datetimeFormats)
            {
                if (DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out dateTime))
                {
                    result = Convert(counterContext, dateTime);
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
            var counterContext = CounterContext.Current;
            if (DateTimeOffset.TryParseExact(value, serializersettings.DateFormatString, null, DateTimeStyles.None, out var dateTimeOffset))
            {
                result = Convert(counterContext, dateTimeOffset);
                return true;
            }

            foreach (var format in datetimeOffsetFormats)
            {
                if (DateTimeOffset.TryParseExact(value, format, null, DateTimeStyles.None, out dateTimeOffset))
                {
                    result = Convert(counterContext, dateTimeOffset);
                    return true;
                }
            }
        }

        result = null;
        return false;
    }
}