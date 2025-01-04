// ReSharper disable ReturnValueOfPureMethodIsNotUsed
static partial class DateScrubber
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
        if (span.ContainsNewline())
        {
            result = null;
            return false;
        }

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
        if (span.ContainsNewline())
        {
            result = null;
            return false;
        }

        if (DateTimeOffsetPolyfill.TryParseExact(span, format, culture, DateTimeStyles.None, out var date))
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
            TryConvertDateTimeOffset);

    static bool TryConvertDateTime(
        CharSpan span,
        [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format,
        Counter counter,
        Culture culture,
        [NotNullWhen(true)] out string? result)
    {
        if (span.ContainsNewline())
        {
            result = null;
            return false;
        }

        if (DateTimePolyfill.TryParseExact(span, format, culture, DateTimeStyles.None, out var date))
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
            TryConvertDateTime);

    static void ReplaceInner(StringBuilder builder, string format, Counter counter, Culture culture, TryConvert tryConvertDate)
    {
        var (max, min) = DateFormatLengthCalculator.GetLength(format, culture);

        if (builder.Length < min)
        {
            return;
        }

        if (min == max)
        {
            ReplaceFixedLength(builder, format, counter, culture, tryConvertDate, max);

            return;
        }

        ReplaceVariableLength(builder, format, counter, culture, tryConvertDate, max, min);
    }

    static void ReplaceVariableLength(StringBuilder builder, string format, Counter counter, Culture culture, TryConvert tryConvertDate, int longest, int shortest)
    {
        var value = builder.AsSpan();
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
                if (tryConvertDate(slice, format, counter, culture, out var convert))
                {
                    builder.Overwrite(convert, builderIndex, length);
                    builderIndex += convert.Length;
                    index += length - 1;
                    found = true;
                    break;
                }
            }

            if (found)
            {
                continue;
            }

            builderIndex++;
        }
    }

    static void ReplaceFixedLength(StringBuilder builder, string format, Counter counter, Culture culture, TryConvert tryConvertDate, int length)
    {
        var value = builder.AsSpan();
        var builderIndex = 0;
        var increment = length - 1;
        for (var index = 0; index <= value.Length - length; index++)
        {
            var slice = value.Slice(index, length);
            if (tryConvertDate(slice, format, counter, culture, out var convert))
            {
                builder.Overwrite(convert, builderIndex, length);
                builderIndex += convert.Length;
                index += increment;
            }
            else
            {
                builderIndex++;
            }
        }
    }

    internal static CultureDate GetCultureDates(Culture culture)
    {
        if (cultureDates.TryGetValue(culture.Name, out var cultureDate) ||
            cultureDates.TryGetValue(culture.TwoLetterISOLanguageName, out cultureDate))
        {
            return cultureDate;
        }

        throw new($"Could not find culture {culture.Name}");
    }
}