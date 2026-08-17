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
        // ticks, not the Second/Millisecond properties, since sub-millisecond ticks leave both of those zero
        var ticks = value.TimeOfDay.Ticks;

        if (ticks == 0)
        {
            return value.ToString("yyyy-MM-dd", Culture.InvariantCulture);
        }

        if (ticks % TimeSpan.TicksPerMinute == 0)
        {
            return value.ToString("yyyy-MM-dd HH:mm", Culture.InvariantCulture);
        }

        if (ticks % TimeSpan.TicksPerSecond == 0)
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
        var ticks = value.TimeOfDay.Ticks;

        if (ticks == 0)
        {
            return value.ToString("yyyy-MM-dd", Culture.InvariantCulture);
        }

        if (ticks % TimeSpan.TicksPerMinute == 0)
        {
            return value.ToString("yyyy-MM-ddTHH-mm", Culture.InvariantCulture);
        }

        if (ticks % TimeSpan.TicksPerSecond == 0)
        {
            return value.ToString("yyyy-MM-ddTHH-mm-ss", Culture.InvariantCulture);
        }

        return value.ToString("yyyy-MM-ddTHH-mm-ss.FFFFFFF", Culture.InvariantCulture);
    }

    // Interpolation formats with the current culture, and NumberFormatInfo.NegativeSign is not "-"
    // everywhere: sv-SE renders U+2212 and ar-SA prefixes U+061C. A negative offset carries that
    // sign into both snapshot content and parameter file names, so the culture is pinned here the
    // same way it is for every date part above
    static string GetDateOffset(DateTimeOffset value)
    {
        var offset = value.Offset;

        if (offset > TimeSpan.Zero)
        {
            if (offset.Minutes == 0)
            {
                return FormattableString.Invariant($"+{offset.TotalHours:0}");
            }

            return FormattableString.Invariant($"+{offset.Hours:0}-{offset.Minutes:00}");
        }

        if (offset < TimeSpan.Zero)
        {
            if (offset.Minutes == 0)
            {
                return FormattableString.Invariant($"{offset.Hours:0}");
            }

            // Minutes is negative too, which is what renders the separator
            return FormattableString.Invariant($"{offset.Hours:0}{offset.Minutes:00}");
        }

        return "+0";
    }
}