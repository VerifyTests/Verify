static class DateFormatter
{
    public static string ToJsonString(DateTimeOffset value)
    {
        var result = GetJsonDatePart(value);
        result += $" {GetDateOffset(value)}";
        return result;
    }

    static string GetJsonDatePart(DateTimeOffset value)
    {
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            return value.ToString("yyyy-MM-dd", Culture.InvariantCulture);
        }

        if (value is {Second: 0, Millisecond: 0})
        {
            return value.ToString("yyyy-MM-dd HH:mm", Culture.InvariantCulture);
        }

        if (value.Millisecond == 0)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss", Culture.InvariantCulture);
        }

        return value.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF", Culture.InvariantCulture);
    }

    public static string ToParameterString(DateTimeOffset value)
    {
        var result = GetParameterDatePart(value);
        result += GetDateOffset(value);

        return result;
    }

    static string GetParameterDatePart(DateTimeOffset value)
    {
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            return value.ToString("yyyy-MM-dd", Culture.InvariantCulture);
        }

        if (value is {Second: 0, Millisecond: 0})
        {
            return value.ToString("yyyy-MM-ddTHH-mm", Culture.InvariantCulture);
        }

        if (value.Millisecond == 0)
        {
            return value.ToString("yyyy-MM-ddTHH-mm-ss", Culture.InvariantCulture);
        }

        return value.ToString("yyyy-MM-ddTHH-mm-ss.FFFFFFF", Culture.InvariantCulture);
    }

    public static string ToJsonString(DateTime value)
    {
        var result = GetJsonDatePart(value);

        if (value.Kind != DateTimeKind.Unspecified)
        {
            result += $" {value.Kind}";
        }

        return result;
    }

    static string GetJsonDatePart(DateTime value)
    {
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            return value.ToString("yyyy-MM-dd", Culture.InvariantCulture);
        }

        if (value is {Second: 0, Millisecond: 0})
        {
            return value.ToString("yyyy-MM-dd HH:mm", Culture.InvariantCulture);
        }

        if (value.Millisecond == 0)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss", Culture.InvariantCulture);
        }

        return value.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF", Culture.InvariantCulture);
    }

    public static string ToParameterString(DateTime value)
    {
        var result = GetParameterDatePart(value);

        if (value.Kind != DateTimeKind.Unspecified)
        {
            result += value.Kind;
        }

        return result;
    }

    static string GetParameterDatePart(DateTime value)
    {
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            return value.ToString("yyyy-MM-dd", Culture.InvariantCulture);
        }

        if (value is {Second: 0, Millisecond: 0})
        {
            return value.ToString("yyyy-MM-ddTHH-mm", Culture.InvariantCulture);
        }

        if (value.Millisecond == 0)
        {
            return value.ToString("yyyy-MM-ddTHH-mm-ss", Culture.InvariantCulture);
        }

        return value.ToString("yyyy-MM-ddTHH-mm-ss.FFFFFFF", Culture.InvariantCulture);
    }

    static string GetDateOffset(DateTimeOffset value)
    {
        var offset = value.Offset;

        if (offset > TimeSpan.Zero)
        {
            if (offset.Minutes == 0)
            {
                return $"+{offset.TotalHours:0}";
            }

            return $"+{offset.Hours:0}-{offset.Minutes:00}";
        }

        if (offset < TimeSpan.Zero)
        {
            if (offset.Minutes == 0)
            {
                return $"{offset.Hours:0}";
            }

            return $"{offset.Hours:0}{offset.Minutes:00}";
        }

        return "+0";
    }
}