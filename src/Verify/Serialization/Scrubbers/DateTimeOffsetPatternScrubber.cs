namespace VerifyTests;

sealed class DateTimeOffsetPatternScrubber : PatternScrubber
{
    readonly string format;
    readonly string? trimmedFormat;
    readonly Culture culture;
    readonly int formatMin;
    readonly int formatMax;
    readonly int trimmedMin;
    readonly int trimmedMax;
    readonly int overallMin;
    readonly int overallMax;

    public DateTimeOffsetPatternScrubber(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture)
    {
        try
        {
            DateTimeOffset.MaxValue.ToString(format, culture);
        }
        catch (FormatException exception)
        {
            throw new($"Format '{format}' is not valid for DateTimeOffset.ToString(format, culture).", exception);
        }
        catch (ArgumentOutOfRangeException)
        {
            // Some calendars (e.g. UmAlQuraCalendar) can't represent DateTimeOffset.MaxValue.
            // The format itself is fine; skip the validation in those cases.
        }

        this.format = format;
        this.culture = culture ?? Culture.CurrentCulture;

        var (max, min) = DateFormatLengthCalculator.GetLength(format, this.culture);
        formatMax = max;
        formatMin = min;
        overallMin = min;
        overallMax = max;

        if (DateScrubberFormat.TryGetTrimmed(format, out var trimmed))
        {
            trimmedFormat = trimmed;
            var (tMax, tMin) = DateFormatLengthCalculator.GetLength(trimmed, this.culture);
            trimmedMax = tMax;
            trimmedMin = tMin;
            overallMin = Math.Min(overallMin, tMin);
            overallMax = Math.Max(overallMax, tMax);
        }
    }

    public override int MinLength => overallMin;
    public override int MaxLength => overallMax;
    public override bool SingleLine => true;

    public override bool TryMatch(
        CharSpan source,
        int position,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out int matchLength,
        [NotNullWhen(true)] out string? replacement)
    {
        matchLength = 0;
        replacement = null;

        if (!counter.ScrubDateTimes)
        {
            return false;
        }

        var available = source.Length - position;

        if (TryParseRange(source, position, available, format, formatMin, formatMax, counter, out matchLength, out replacement))
        {
            return true;
        }

        if (trimmedFormat != null &&
            TryParseRange(source, position, available, trimmedFormat, trimmedMin, trimmedMax, counter, out matchLength, out replacement))
        {
            return true;
        }

        matchLength = 0;
        replacement = null;
        return false;
    }

    bool TryParseRange(
        CharSpan source,
        int position,
        int available,
        string fmt,
        int min,
        int max,
        Counter counter,
        out int matchLength,
        [NotNullWhen(true)] out string? replacement)
    {
        var maxLen = available < max ? available : max;
        for (var len = maxLen; len >= min; len--)
        {
            var slice = source.Slice(position, len);
            if (DateTimeOffset.TryParseExact(slice, fmt, culture, DateTimeStyles.None, out var date))
            {
                matchLength = len;
                replacement = counter.Convert(date);
                return true;
            }
        }

        matchLength = 0;
        replacement = null;
        return false;
    }
}
