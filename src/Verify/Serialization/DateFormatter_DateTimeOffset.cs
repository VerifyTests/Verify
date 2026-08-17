namespace VerifyTests;

public static partial class DateFormatter
{
    public static string Convert(DateTimeOffset value)
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

    static string GetDateOffset(DateTimeOffset value)
    {
        var offset = value.Offset;

        if (offset > TimeSpan.Zero)
        {
            if (offset.Minutes == 0)
            {
                return $"+{offset.TotalHours.ToString("0", Culture.InvariantCulture)}";
            }

            return $"+{offset.Hours.ToString("0", Culture.InvariantCulture)}-{offset.Minutes.ToString("00", Culture.InvariantCulture)}";
        }

        if (offset < TimeSpan.Zero)
        {
            if (offset.Minutes == 0)
            {
                return offset.Hours.ToString("0", Culture.InvariantCulture);
            }

            return $"{offset.Hours.ToString("0", Culture.InvariantCulture)}{offset.Minutes.ToString("00", Culture.InvariantCulture)}";
        }

        return "+0";
    }
}