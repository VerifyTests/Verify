// ReSharper disable ReturnValueOfPureMethodIsNotUsed
static class DateScrubber
{
    delegate bool TryConvert(
        CharSpan span,
        string format,
        Counter counter,
        Culture culture,
        [NotNullWhen(true)] out string? result);

#if NET6_0_OR_GREATER

    static bool TryConvertDate(
        CharSpan span,
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Counter counter,
        Culture culture,
        [NotNullWhen(true)] out string? result)
    {
        if (Date.TryParseExact(span, format, culture, DateTimeStyles.None, out var date))
        {
            result = counter.Convert(date);
            return true;
        }

        result = null;
        return false;
    }

    public static Action<StringBuilder, Counter> BuildDateScrubber(
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

        return (builder, counter) => ReplaceDates(builder, format, counter, culture ?? Culture.CurrentCulture);
    }

    public static void ReplaceDates(
        StringBuilder builder,
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Counter counter,
        Culture culture) =>
        ReplaceInner(
            builder,
            format,
            counter,
            culture,
            TryConvertDate);
#endif

    public static Action<StringBuilder, Counter> BuildDateTimeOffsetScrubber(
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

        return (builder, counter) => ReplaceDateTimeOffsets(builder, format, counter, culture ?? Culture.CurrentCulture);
    }

    static bool TryConvertDateTimeOffset(
        CharSpan span,
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Counter counter,
        Culture culture,
        [NotNullWhen(true)] out string? result)
    {
        if (DateTimeOffset.TryParseExact(span, format, culture, DateTimeStyles.None, out var date))
        {
            result = counter.Convert(date);
            return true;
        }

        result = null;
        return false;
    }

    public static void ReplaceDateTimeOffsets(
        StringBuilder builder,
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Counter counter,
        Culture culture)
    {
        ReplaceInner(
            builder,
            format,
            counter,
            culture,
            TryConvertDateTimeOffset);
        if (TryGetFormatWithUpperMillisecondsTrimmed(format, out var trimmedFormat))
        {
            ReplaceInner(
                builder,
                trimmedFormat,
                counter,
                culture,
                TryConvertDateTimeOffset);
        }
    }

    static bool TryConvertDateTime(
        CharSpan span,
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Counter counter,
        Culture culture,
        [NotNullWhen(true)] out string? result)
    {
        if (DateTime.TryParseExact(span, format, culture, DateTimeStyles.None, out var date))
        {
            result = counter.Convert(date);
            return true;
        }

        result = null;
        return false;
    }

    public static Action<StringBuilder, Counter> BuildDateTimeScrubber(string format, Culture? culture)
    {
        try
        {
            DateTime.MaxValue.ToString(format, culture);
        }
        catch (FormatException exception)
        {
            throw new($"Format '{format}' is not valid for DateTime.ToString(format, culture).", exception);
        }

        return (builder, counter) => ReplaceDateTimes(builder, format, counter, culture ?? Culture.CurrentCulture);
    }

    public static void ReplaceDateTimes(StringBuilder builder, string format, Counter counter, Culture culture)
    {
        ReplaceInner(
            builder,
            format,
            counter,
            culture,
            TryConvertDateTime);
        if (TryGetFormatWithUpperMillisecondsTrimmed(format, out var trimmedFormat))
        {
            ReplaceInner(
                builder,
                trimmedFormat,
                counter,
                culture,
                TryConvertDateTime);
        }
    }

    static bool TryGetFormatWithUpperMillisecondsTrimmed(string format, [NotNullWhen(true)] out string? trimmedFormat)
    {
        if (format.EndsWith(".FFFF"))
        {
            trimmedFormat = format[..^5];
            return true;
        }

        if (format.EndsWith(".FFF"))
        {
            trimmedFormat = format[..^4];
            return true;
        }

        if (format.EndsWith(".FF"))
        {
            trimmedFormat = format[..^3];
            return true;
        }

        if (format.EndsWith(".F"))
        {
            trimmedFormat = format[..^2];
            return true;
        }

        trimmedFormat = null;
        return false;
    }

    static void ReplaceInner(StringBuilder builder, string format, Counter counter, Culture culture, TryConvert tryConvert)
    {
        if (!counter.ScrubDateTimes)
        {
            return;
        }

        var (max, min) = DateFormatLengthCalculator.GetLength(format, culture);

        if (builder.Length < min)
        {
            return;
        }

        var context = new MatchContext(format, counter, culture, tryConvert, max, min);

        CrossChunkMatcher.ReplaceAll<MatchContext>(
            builder,
            carryoverSize: max - 1,
            context,
            OnCrossChunk,
            OnWithinChunk,
            getMatches: c => c.Matches,
            getLength: m => m.Length,
            getValue: m => m.Value);
    }

    static void OnCrossChunk(
        StringBuilder builder,
        Span<char> carryoverBuffer,
        int carryoverIndex,
        int remainingInCarryover,
        CharSpan currentChunkSpan,
        int absoluteStartPosition,
        MatchContext context)
    {
        Span<char> combinedBuffer = stackalloc char[context.MaxLength];

        // Try lengths from longest to shortest (greedy matching)
        for (var length = context.MaxLength; length >= context.MinLength; length--)
        {
            var neededFromCurrent = length - remainingInCarryover;

            if (neededFromCurrent <= 0 ||
                neededFromCurrent > currentChunkSpan.Length)
            {
                continue;
            }

            // Combine carryover and current chunk
            carryoverBuffer.Slice(carryoverIndex, remainingInCarryover).CopyTo(combinedBuffer);
            currentChunkSpan[..neededFromCurrent].CopyTo(combinedBuffer[remainingInCarryover..]);

            var slice = combinedBuffer[..length];

            if (context.TryConvert(slice, context.Format, context.Counter, context.Culture, out var convert))
            {
                context.Matches.Add(new(absoluteStartPosition, length, convert));
                return; // Found match at this position
            }
        }
    }

    static int OnWithinChunk(
        ReadOnlyMemory<char> chunk,
        CharSpan chunkSpan,
        int chunkIndex,
        int absoluteIndex,
        MatchContext context)
    {
        // Try lengths from longest to shortest (greedy matching)
        for (var length = context.MaxLength; length >= context.MinLength; length--)
        {
            if (chunkIndex + length > chunk.Length)
            {
                continue;
            }

            var slice = chunkSpan.Slice(chunkIndex, length);

            if (context.TryConvert(slice, context.Format, context.Counter, context.Culture, out var convert))
            {
                context.Matches.Add(new(absoluteIndex, length, convert));
                return length; // Skip past match
            }
        }

        return 1;
    }

    sealed class MatchContext(
        string format,
        Counter counter,
        Culture culture,
        TryConvert tryConvert,
        int maxLength,
        int minLength)
    {
        public string Format { get; } = format;
        public Counter Counter { get; } = counter;
        public Culture Culture { get; } = culture;
        public TryConvert TryConvert { get; } = tryConvert;
        public int MaxLength { get; } = maxLength;
        public int MinLength { get; } = minLength;
        public List<Match> Matches { get; } = [];
    }
}
