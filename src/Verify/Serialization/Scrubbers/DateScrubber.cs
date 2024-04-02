static partial class DateScrubber
{
    delegate bool TryConvert(CharSpan span, string format, Counter counter, Culture culture, [NotNullWhen(true)] out string? result);

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
            result = SerializationSettings.Convert(counter, date);
            return true;
        }

        result = null;
        return false;
    }

    public static Action<StringBuilder, Counter> BuildDateScrubber(
        [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format,
        Culture? culture)
    {
        culture ??= Culture.CurrentCulture;
        return (builder, counter) => ReplaceDates(builder, format, counter, culture);
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
            _ => Date.FromDateTime(_),
            TryConvertDate);
#endif

    public static Action<StringBuilder, Counter> BuildDateTimeOffsetScrubber(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture)
    {
        culture ??= Culture.CurrentCulture;
        return (builder, counter) => ReplaceDateTimeOffsets(builder, format, counter, culture);
    }

    static bool TryConvertDateTimeOffset(
        CharSpan span,
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Counter counter,
        Culture culture,
        [NotNullWhen(true)] out string? result)
    {
#if NET5_0_OR_GREATER
        if (DateTimeOffset.TryParseExact(span, format, culture, DateTimeStyles.None, out var date))
#else
        if (DateTimeOffset.TryParseExact(span.ToString(), format, culture, DateTimeStyles.None, out var date))
#endif
        {
            result = SerializationSettings.Convert(counter, date);
            return true;
        }

        result = null;
        return false;
    }

    public static void ReplaceDateTimeOffsets(
        StringBuilder builder,
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Counter counter,
        Culture culture) =>
        ReplaceInner(
            builder,
            format,
            counter,
            culture,
            _ => new DateTimeOffset(_),
            TryConvertDateTimeOffset);

    static bool TryConvertDateTime(
        CharSpan span,
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Counter counter,
        Culture culture,
        [NotNullWhen(true)] out string? result)
    {
#if NET5_0_OR_GREATER
        if (DateTime.TryParseExact(span, format, culture, DateTimeStyles.None, out var date))
#else
        if (DateTime.TryParseExact(span.ToString(), format, culture, DateTimeStyles.None, out var date))
#endif
        {
            result = SerializationSettings.Convert(counter, date);
            return true;
        }

        result = null;
        return false;
    }

    public static Action<StringBuilder, Counter> BuildDateTimeScrubber(
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Culture? culture)
    {
        culture ??= Culture.CurrentCulture;
        return (builder, counter) => ReplaceDateTimes(builder, format, counter, culture);
    }

    public static void ReplaceDateTimes(
        StringBuilder builder,
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Counter counter,
        Culture culture) =>
        ReplaceInner(
            builder,
            format,
            counter,
            culture,
            _ => _,
            TryConvertDateTime);

    static void ReplaceInner(StringBuilder builder, string format, Counter counter, Culture culture, Func<DateTime, IFormattable> toDate, TryConvert tryConvertDate)
    {
        int Length(DateTime dateTime)
        {
            var date = toDate(dateTime);
            try
            {
                return date.ToString(format, culture).Length;
            }
            catch (Exception exception)
            {
                throw new($"Failed to get length for {date.GetType()} {date.ToString()} using format '{format}' and culture {culture}.", exception);
            }
        }

        var cultureDate = GetCultureDates(culture);
        var value = builder.AsSpan();
        var shortest = Length(cultureDate.Short);
        var longest = Length(cultureDate.Long);

        var builderIndex = 0;
        for (var index = 0; index <= value.Length; index++)
        {
            var found = false;
            for (var length = longest; length >= shortest; length--)
            {
                var end = index + length;
                if (end > value.Length)
                {
                    continue;
                }

                var slice = value.Slice(index, length);
                if (!slice.ContainsNewline())
                {
                    if (tryConvertDate(slice, format, counter, culture, out var convert))
                    {
                        builder.Overwrite(convert, builderIndex, length);
                        builderIndex += convert.Length;
                        index += length - 1;
                        found = true;
                        break;
                    }
                }
            }

            if (found)
            {
                continue;
            }

            builderIndex++;
        }
    }

    internal static CultureDate GetCultureDates(Culture culture)
    {
        if (!cultureDates.TryGetValue(culture.Name, out var cultureDate) &&
            !cultureDates.TryGetValue(culture.TwoLetterISOLanguageName, out cultureDate))
        {
            throw new($"Could not find culture {culture.Name}");
        }

        return cultureDate;
    }
}