static partial class DateScrubber
{
    delegate bool TryConvert(CharSpan span, string format, Counter counter, Culture culture, [NotNullWhen(true)] out string? result);

#if NET6_0_OR_GREATER

    static bool TryConvertDate(CharSpan span, string format, Counter counter, Culture culture, [NotNullWhen(true)] out string? result)
    {
        if (Date.TryParseExact(span, format, culture, DateTimeStyles.None, out var date))
        {
            result = SerializationSettings.Convert(counter, date);
            return true;
        }

        result = null;
        return false;
    }

    public static void ReplaceDates(StringBuilder builder, string format, Counter counter, Culture culture) =>
        ReplaceInner(
            builder,
            format,
            counter,
            culture,
            _ => Date.FromDateTime(_),
            TryConvertDate);
#endif

    static bool TryConvertDateTimeOffset(CharSpan span, string format, Counter counter, Culture culture, [NotNullWhen(true)] out string? result)
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

    public static void ReplaceDateTimeOffsets(StringBuilder builder, string format, Counter counter, Culture culture) =>
        ReplaceInner(
            builder,
            format,
            counter,
            culture,
            _ => new DateTimeOffset(_),
            TryConvertDateTimeOffset);

    static bool TryConvertDateTime(CharSpan span, string format, Counter counter, Culture culture, [NotNullWhen(true)] out string? result)
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

    public static void ReplaceDateTimes(StringBuilder builder, string format, Counter counter, Culture culture) =>
        ReplaceInner(
            builder,
            format,
            counter,
            culture,
            _ => _,
            TryConvertDateTime);

    static void ReplaceInner(StringBuilder builder, string format, Counter counter, Culture culture, Func<DateTime, IFormattable> toDate, TryConvert tryConvertDate)
    {
        var cultureDate = GetCultureDates(culture);
        var value = builder.AsSpan();
        var longDate = toDate(cultureDate.Long);
        var shortDate = toDate(cultureDate.Short);
        var longest = longDate.ToString(format, culture)
            .Length;
        var shortest = shortDate.ToString(format, culture)
            .Length;

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
        if (!cultureDates.TryGetValue(culture.Name, out var cultureDate))
        {
            if (!cultureDates.TryGetValue(culture.TwoLetterISOLanguageName, out cultureDate))
            {
                throw new($"Could not find culture {culture.Name}");
            }
        }

        return cultureDate;
    }
}