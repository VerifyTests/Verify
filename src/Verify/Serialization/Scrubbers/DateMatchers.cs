// The inline date scrubbers: window scrubbers whose bounds come from
// DateFormatLengthCalculator and whose matcher is a TryParseExact for the format.
// Formats ending in upper case fraction specifiers (.F to .FFFF) produce a second
// scrubber for the trimmed format, since those fractions render as empty when zero;
// the longer max of the untrimmed scrubber naturally orders it first.
// The culture (CurrentCulture when null) is resolved when the scrubber is created.
static class DateMatchers
{
    // A probe date within the supported range of every calendar
    // (DateTime.MaxValue is out of range for e.g. the UmAlQura calendar)
    static DateTime probeDateTime = new(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc);

    public static Scrubber[] DateTimes(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture)
    {
        var resolvedCulture = culture ?? Culture.CurrentCulture;
        try
        {
            probeDateTime.ToString(format, resolvedCulture);
        }
        catch (FormatException exception)
        {
            throw new($"Format '{format}' is not valid for DateTime.ToString(format, culture).", exception);
        }

        return BuildForFormats(
            format,
            resolvedCulture,
            static (format, culture) => Single(
                format,
                culture,
                (window, counter) =>
                {
                    if (DateTime.TryParseExact(window, format, culture, DateTimeStyles.None, out var date))
                    {
                        return counter.Convert(date);
                    }

                    return null;
                }));
    }

    public static Scrubber[] DateTimeOffsets(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture)
    {
        var resolvedCulture = culture ?? Culture.CurrentCulture;
        try
        {
            new DateTimeOffset(probeDateTime).ToString(format, resolvedCulture);
        }
        catch (FormatException exception)
        {
            throw new($"Format '{format}' is not valid for DateTimeOffset.ToString(format, culture).", exception);
        }

        return BuildForFormats(
            format,
            resolvedCulture,
            static (format, culture) => Single(
                format,
                culture,
                (window, counter) =>
                {
                    if (DateTimeOffset.TryParseExact(window, format, culture, DateTimeStyles.None, out var date))
                    {
                        return counter.Convert(date);
                    }

                    return null;
                }));
    }

#if NET6_0_OR_GREATER

    public static Scrubber[] Dates(
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Culture? culture)
    {
        var resolvedCulture = culture ?? Culture.CurrentCulture;
        try
        {
            Date.FromDateTime(probeDateTime).ToString(format, resolvedCulture);
        }
        catch (FormatException exception)
        {
            throw new($"Format '{format}' is not valid for DateOnly.ToString(format, culture).", exception);
        }

        return BuildForFormats(
            format,
            resolvedCulture,
            static (format, culture) => Single(
                format,
                culture,
                (window, counter) =>
                {
                    if (Date.TryParseExact(window, format, culture, DateTimeStyles.None, out var date))
                    {
                        return counter.Convert(date);
                    }

                    return null;
                }));
    }

#endif

    delegate string? ParseWindow(CharSpan window, Counter counter);

    static Scrubber[] BuildForFormats(string format, Culture culture, Func<string, Culture, Scrubber> build)
    {
        if (TryGetFormatWithUpperMillisecondsTrimmed(format, out var trimmedFormat))
        {
            return
            [
                build(format, culture),
                build(trimmedFormat, culture)
            ];
        }

        return [build(format, culture)];
    }

    static Scrubber Single(string format, Culture culture, ParseWindow parse)
    {
        var (max, min) = DateFormatLengthCalculator.GetLength(format, culture);
        // Single char standard formats expand to the culture's pattern, so the
        // prefilter analysis runs on what will actually render
        var digitPrefilter = HasDigitPrefilter(culture.DateTimeFormat.ExpandFormat(format));

        string? Match(CharSpan window, Counter counter, IReadOnlyDictionary<string, object> context)
        {
            if (!counter.ScrubDateTimes)
            {
                return null;
            }

            return parse(window, counter);
        }

        if (digitPrefilter)
        {
            // The engine anchor jumps between digits, so no-match text is scanned
            // vectorized instead of per position
            return Scrubber.AnchoredWindow(
                Math.Max(1, min),
                max,
                Match,
                requireWordBoundary: false,
                anchor: WindowAnchor.Digit,
                anchorChar: default,
                anchorOffset: 0);
        }

        return Scrubber.Window(
            Math.Max(1, min),
            max,
            Match);
    }

    // True when the (expanded) format is guaranteed to render a digit first,
    // allowing the parse to be skipped cheaply. Formats starting with a name,
    // era, or literal token are not prefiltered.
    static bool HasDigitPrefilter(string format)
    {
        if (format.Length < 2)
        {
            return false;
        }

        var first = format[0];
        switch (first)
        {
            case 'y' or 'H' or 'h' or 'm' or 's' or 'f' or 'F':
                return true;
            case 'd' or 'M':
                // 1 or 2 repeats render digits; 3+ render names
                return format.Length < 3 ||
                       format[1] != first ||
                       format[2] != first;
            default:
                return false;
        }
    }

    static bool TryGetFormatWithUpperMillisecondsTrimmed(string format, [NotNullWhen(true)] out string? trimmedFormat)
    {
        if (format.EndsWith(".FFFF", StringComparison.Ordinal))
        {
            trimmedFormat = format[..^5];
            return true;
        }

        if (format.EndsWith(".FFF", StringComparison.Ordinal))
        {
            trimmedFormat = format[..^4];
            return true;
        }

        if (format.EndsWith(".FF", StringComparison.Ordinal))
        {
            trimmedFormat = format[..^3];
            return true;
        }

        if (format.EndsWith(".F", StringComparison.Ordinal))
        {
            trimmedFormat = format[..^2];
            return true;
        }

        trimmedFormat = null;
        return false;
    }
}
