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

        stringValue += $" {GetOffsetOrKind(value)}";

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

        stringValue += GetOffsetOrKind(value);

        return stringValue;
    }

    static string GetOffsetOrKind(DateTime value)
    {
        if (value.Kind == DateTimeKind.Unspecified)
        {
            var offset = TimeZoneInfo.Local.GetUtcOffset(value);
            return GetOffsetText(offset);
        }

        return value.Kind.ToString();
    }

    static TimeSpan machineOffset = DateTimeOffset.Now.Offset;
    static string GetDateOffset(DateTimeOffset value)
    {
        var offset = value.Offset;
        if (offset == TimeSpan.Zero)
        {
            return "Utc";
        }

        if (offset == machineOffset)
        {
            return "Local";
        }

        return GetOffsetText(offset);
    }

    static string GetOffsetText(TimeSpan offset)
    {
        var hours = offset.Hours;
        var minutes = offset.Minutes;

        if (offset > TimeSpan.Zero)
        {
            if (minutes == 0)
            {
                return $"+{hours}";
            }

            return $"+{hours}:{minutes}";
        }

        if (minutes == 0)
        {
            return $"-{hours}";
        }

        return $"-{hours}:{minutes}";
    }
}