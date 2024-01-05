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

    public static Action<StringBuilder, Counter> BuildDateScrubber(string format, Culture? culture)
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

    public static void ReplaceDates(StringBuilder builder, string format, Counter counter, Culture culture) =>
        ReplaceInner(
            builder,
            format,
            counter,
            culture,
            _ => Date.FromDateTime(_),
            TryConvertDate);
#endif

    public static Action<StringBuilder, Counter> BuildDateTimeOffsetScrubber(string format, Culture? culture)
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