static class DateFormatter
{
    public static string ToJsonString(DateTimeOffset value, bool offset)
    {
        var result = GetJsonDatePart(value);
        if (offset)
        {
            result += $" {GetDateOffset(value)}";
        }

        return result;
    }

    static string GetJsonDatePart(DateTimeOffset value)
    {
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        if (value.Second == 0 && value.Millisecond == 0)
        {
            return value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        }

        if (value.Millisecond == 0)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        return value.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture);
    }

    public static string ToParameterString(DateTimeOffset value, bool offset)
    {
        var result = GetParameterDatePart(value);
        if (offset)
        {
            result += GetDateOffset(value);
        }

        return result;
    }

    static string GetParameterDatePart(DateTimeOffset value)
    {
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        if (value.Second == 0 && value.Millisecond == 0)
        {
            return value.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
        }

        if (value.Millisecond == 0)
        {
            return value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }

        return value.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture);
    }

    public static string ToJsonString(DateTime value, bool kind, bool offset)
    {
        var result = GetJsonDatePart(value);
        if (offset)
        {
            result += $" {GetDateOffset(value)}";
        }

        if (kind)
        {
            result += $" {value.Kind}";
        }

        return result;
    }

    static string GetJsonDatePart(DateTime value)
    {
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        if (value.Second == 0 && value.Millisecond == 0)
        {
            return value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        }

        if (value.Millisecond == 0)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        return value.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture);
    }

    public static string ToParameterString(DateTime value, bool kind, bool offset)
    {
        var result = GetParameterDatePart(value);
        if (offset)
        {
            result += GetDateOffset(value);
        }

        if (kind)
        {
            result += value.Kind;
        }

        return result;
    }

    static string GetParameterDatePart(DateTime value)
    {
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        if (value.Second == 0 && value.Millisecond == 0)
        {
            return value.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
        }

        if (value.Millisecond == 0)
        {
            return value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }

        return value.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture);
    }

    static string GetDateOffset(IFormattable value) =>
        value.ToString("zzz", CultureInfo.InvariantCulture)
            .Replace(":00", "")
            .Replace("+0", "+")
            .Replace("-0", "-");
}