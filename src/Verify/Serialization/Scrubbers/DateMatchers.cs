// The inline date scrubbers: window scrubbers whose bounds come from
// DateFormatLengthCalculator and whose matcher is a TryParseExact for the format.
// Formats ending in upper case fraction specifiers (.F to .FFFF) produce a second
// scrubber for the trimmed format, since those fractions render as empty when zero;
// the longer max of the untrimmed scrubber naturally orders it first.
// An explicit culture is resolved when the scrubber is created. When none is given
// the culture in effect at scrub time is used, resolved (and cached) per culture on
// each scrub, since the parse, the window bounds, and the anchor all depend on it.
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
            culture,
            resolvedCulture,
            static (format, culture) => (window, counter) =>
            {
                if (DateTime.TryParseExact(window, format, culture, DateTimeStyles.None, out var date))
                {
                    return counter.Convert(date);
                }

                return null;
            });
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
            culture,
            resolvedCulture,
            static (format, culture) => (window, counter) =>
            {
                if (DateTimeOffset.TryParseExact(window, format, culture, DateTimeStyles.None, out var date))
                {
                    return counter.Convert(date);
                }

                return null;
            });
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
            culture,
            resolvedCulture,
            static (format, culture) => (window, counter) =>
            {
                if (Date.TryParseExact(window, format, culture, DateTimeStyles.None, out var date))
                {
                    return counter.Convert(date);
                }

                return null;
            });
    }

#endif

    delegate string? ParseWindow(CharSpan window, Counter counter);

    // Builds the parse for one format and culture pair
    delegate ParseWindow ParseFactory(string format, Culture culture);

    static Scrubber[] BuildForFormats(
        string format,
        Culture? culture,
        Culture registrationCulture,
        ParseFactory parseFactory)
    {
        if (TryGetFormatWithUpperMillisecondsTrimmed(format, out var trimmedFormat))
        {
            return
            [
                ForCulture(format, culture, registrationCulture, parseFactory),
                ForCulture(trimmedFormat, culture, registrationCulture, parseFactory)
            ];
        }

        return [ForCulture(format, culture, registrationCulture, parseFactory)];
    }

    static Scrubber ForCulture(
        string format,
        Culture? culture,
        Culture registrationCulture,
        ParseFactory parseFactory)
    {
        if (culture != null)
        {
            return Single(format, culture, parseFactory(format, culture));
        }

        // No culture was supplied, so each scrub uses the culture in effect at that
        // point. Building a scrubber reads the format lengths and expands the
        // pattern, so the result is cached per culture.
        ConcurrentDictionary<Culture, Scrubber> cache = new();

        Scrubber ForCurrentCulture()
        {
            var current = Culture.CurrentCulture;
            if (cache.TryGetValue(current, out var existing))
            {
                return existing;
            }

            return cache.GetOrAdd(current, Single(format, current, parseFactory(format, current)));
        }

        // The registration culture instance supplies the bounds used for ordering
        // and the length fast path; the resolver supplies the one actually run
        return Single(
            format,
            registrationCulture,
            parseFactory(format, registrationCulture),
            ForCurrentCulture);
    }

    static Scrubber Single(string format, Culture culture, ParseWindow parse, Func<Scrubber>? resolver = null)
    {
        var (max, min) = DateFormatLengthCalculator.GetLength(format, culture);
        var digitPrefilter = HasDigitPrefilter(format, culture);

        string? Match(CharSpan window, Counter counter, IReadOnlyDictionary<string, object> context) =>
            parse(window, counter);

        if (digitPrefilter)
        {
            // The engine anchor jumps between digits, so no-match text is scanned
            // vectorized instead of per position
            return Scrubber.GatedWindow(
                Math.Max(1, min),
                max,
                Match,
                static counter => counter.ScrubDateTimes,
                anchor: WindowAnchor.Digit,
                resolver: resolver);
        }

        return Scrubber.GatedWindow(
            Math.Max(1, min),
            max,
            Match,
            static counter => counter.ScrubDateTimes,
            resolver: resolver);
    }

    // Probe dates for the digit prefilter: a zero fraction and a non zero one,
    // single and double digit components, AM and PM. All sit within the supported
    // range of every calendar.
    static DateTime[] prefilterProbes =
    [
        new(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc),
        new(2000, 12, 25, 23, 59, 59, 999, DateTimeKind.Utc),
        new(2001, 6, 15, 11, 30, 5, DateTimeKind.Utc)
    ];

    // True when the format renders an ASCII digit first, letting the engine jump
    // between digit positions instead of probing every one. The format grammar is
    // only a necessary condition, so it is confirmed by rendering: that excludes
    // tokens which can render empty (upper case F with a zero fraction) and
    // calendars that render numbers as letters (for example Hebrew).
    static bool HasDigitPrefilter(string format, Culture culture)
    {
        // Single char standard formats expand to the culture's pattern, so the
        // grammar check runs on what will actually render
        if (!StartsWithNumericToken(culture.DateTimeFormat.ExpandFormat(format)))
        {
            return false;
        }

        foreach (var probe in prefilterProbes)
        {
            string rendered;
            try
            {
                rendered = probe.ToString(format, culture);
            }
            catch (FormatException)
            {
                return false;
            }

            if (rendered.Length == 0 ||
                !IsAsciiDigit(rendered[0]))
            {
                return false;
            }
        }

        return true;
    }

    static bool IsAsciiDigit(char value) =>
        value is >= '0' and <= '9';

    // True when the (expanded) format starts with a token that renders a number.
    // Formats starting with a name, era, or literal token are not prefiltered.
    // Upper case F is excluded since it renders nothing when the fraction is zero.
    static bool StartsWithNumericToken(string format)
    {
        if (format.Length < 2)
        {
            return false;
        }

        var first = format[0];
        switch (first)
        {
            case 'y' or 'H' or 'h' or 'm' or 's' or 'f':
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
