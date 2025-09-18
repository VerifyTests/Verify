namespace VerifyTests;

public static partial class DateFormatter
{
    public static string Convert(DateTime value)
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
}