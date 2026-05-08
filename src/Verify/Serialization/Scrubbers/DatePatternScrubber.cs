#if NET6_0_OR_GREATER

namespace VerifyTests;

sealed class DatePatternScrubber : PatternScrubber
{
    readonly string format;
    readonly Culture culture;
    readonly int formatMin;
    readonly int formatMax;

    public DatePatternScrubber(
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Culture? culture)
    {
        try
        {
            Date.MaxValue.ToString(format, culture);
        }
        catch (FormatException exception)
        {
            throw new($"Format '{format}' is not valid for DateOnly.ToString(format, culture).", exception);
        }
        catch (ArgumentOutOfRangeException)
        {
            // Some calendars (e.g. UmAlQuraCalendar) can't represent Date.MaxValue.
            // The format itself is fine; skip the validation in those cases.
        }

        this.format = format;
        this.culture = culture ?? Culture.CurrentCulture;

        var (max, min) = DateFormatLengthCalculator.GetLength(format, this.culture);
        formatMax = max;
        formatMin = min;
    }

    public override int MinLength => formatMin;
    public override int MaxLength => formatMax;
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
        var maxLen = available < formatMax ? available : formatMax;
        for (var len = maxLen; len >= formatMin; len--)
        {
            var slice = source.Slice(position, len);
            if (Date.TryParseExact(slice, format, culture, DateTimeStyles.None, out var date))
            {
                matchLength = len;
                replacement = counter.Convert(date);
                return true;
            }
        }

        return false;
    }
}

#endif
