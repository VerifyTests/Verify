namespace VerifyTests;

static class DateFormatter
{
    public static string ToJsonString(DateTimeOffset value)
    {
        string stringValue;
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            stringValue = value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        else
        {
            if (value.Second == 0 && value.Millisecond == 0)
            {
                stringValue = value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            }
            else if (value.Millisecond == 0)
            {
                stringValue = value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            else
            {
                stringValue = value.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture);
            }
        }

        if (value.Offset != TimeSpan.Zero)
        {
            stringValue += $" {GetDateOffset(value)}";
        }

        return stringValue;
    }

    public static string ToParameterString(DateTimeOffset value)
    {
        string stringValue;
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            stringValue = value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        else
        {
            if (value.Second == 0 && value.Millisecond == 0)
            {
                stringValue = value.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            }
            else if (value.Millisecond == 0)
            {
                stringValue = value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            }
            else
            {
                stringValue = value.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture);
            }
        }

        if (value.Offset != TimeSpan.Zero)
        {
            stringValue += GetDateOffset(value);
        }

        return stringValue;
    }

    public static string ToJsonString(DateTime value)
    {
        string stringValue;
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            stringValue = value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        else
        {
            if (value.Second == 0 && value.Millisecond == 0)
            {
                stringValue = value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            }
            else if (value.Millisecond == 0)
            {
                stringValue = value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            else
            {
                stringValue = value.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture);
            }
        }

        if (value.Kind != DateTimeKind.Utc)
        {
            stringValue += $" {GetDateOffset(value)}";
        }

        if (value.Kind != DateTimeKind.Unspecified)
        {
            stringValue += $" {value.Kind}";
        }
        return stringValue;
    }

    public static string ToParameterString(DateTime value)
    {
        string stringValue;
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            stringValue = value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        else
        {
            if (value.Second == 0 && value.Millisecond == 0)
            {
                stringValue = value.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            }
            else if (value.Millisecond == 0)
            {
                stringValue = value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            }
            else
            {
                stringValue = value.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture);
            }
        }

        if (value.Kind != DateTimeKind.Utc)
        {
            stringValue += GetDateOffset(value);
        }

        if (value.Kind != DateTimeKind.Unspecified)
        {
            stringValue += value.Kind;
        }

        return stringValue;
    }

    static string GetDateOffset(IFormattable value) =>
        value.ToString("zzz", CultureInfo.InvariantCulture)
            .Replace(":00", "")
            .Replace("+0", "+")
            .Replace("-0", "-");
}