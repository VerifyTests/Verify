readonly struct CultureDate(DateTime longDate, DateTime shortDate)
{
    public DateTime Long { get; } = longDate;
    public DateTime Short { get; } = shortDate;
}
static partial class DateScrubber
{
    delegate bool TryConvert(CharSpan span, string format, Counter counter, [NotNullWhen(true)] out string? result);

#if NET6_0_OR_GREATER

    static bool TryConvertDate(CharSpan span, string format, Counter counter, [NotNullWhen(true)] out string? result)
    {
        if (Date.TryParseExact(span, format, out var date))
        {
            result = SerializationSettings.Convert(counter, date);
            return true;
        }

        result = null;
        return false;
    }
    public static void ReplaceDates(StringBuilder builder, string format, Counter counter, Culture culture)
    {
        var value = builder.AsSpan();

        var cultureDate = GetCultureDates(culture);

        var longest = Date.FromDateTime(cultureDate.Long).ToString(format).Length;
        var shortest = Date.FromDateTime(cultureDate.Short).ToString(format).Length;

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
                    if (TryConvertDate(slice, format,counter, out var convert))
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

    static CultureDate GetCultureDates(Culture culture)
    {
        if (!cultureDates.TryGetValue(culture.Name, out var cultureDate))
        {
            throw new($"Could not find culture {culture.Name}");
        }

        return cultureDate;
    }
#endif
}