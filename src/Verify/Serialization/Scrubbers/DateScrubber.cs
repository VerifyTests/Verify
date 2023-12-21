readonly struct CultureDate(DateTime longDate, DateTime shortDate)
{
    public DateTime Long { get; } = longDate;
    public DateTime Short { get; } = shortDate;
}
static partial class DateScrubber
{
#if NET6_0_OR_GREATER

    public static void ReplaceDates(StringBuilder builder, string format, Counter counter, Culture culture)
    {
        var value = builder
            .ToString()
            .AsSpan();

        if (!cultureDates.TryGetValue(culture.Name, out var cultureDate))
        {
            throw new($"Could not find culture {culture.Name}");
        }

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
                if (!slice.ContainsNewline() &&
                    Date.TryParseExact(slice, format, out var date))
                {
                    var convert = SerializationSettings.Convert(counter, date);
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
#endif
}