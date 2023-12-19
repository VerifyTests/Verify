#if NET6_0_OR_GREATER
static class DateScrubber
{
    static Date longDate = new(2023, 12, 20);

    public static IEnumerable<int> Lengths(string format)
    {
        var longLength = longDate.ToString(format, CultureInfo.InvariantCulture)
            .Length;
        var shortLength = Date.MinValue.ToString(format, CultureInfo.InvariantCulture)
            .Length;
        for (var i = longLength; i >= shortLength; i--)
        {
            yield return i;
        }
    }

    public static void ReplaceDates(StringBuilder builder, Counter counter)
    {
        var format = "yyyy-MM-dd";
        var value = builder
            .ToString()
            .AsSpan();

        var builderIndex = 0;
        for (var index = 0; index <= value.Length; index++)
        {
            var end = index + format.Length;
            if (end > value.Length)
            {
                return;
            }

            var slice = value.Slice(index, format.Length);
            if (!slice.ContainsNewline() &&
                Date.TryParseExact(slice, format, out var date))
            {
                var convert = SerializationSettings.Convert(counter, date);
                builder.Overwrite(convert, builderIndex, format.Length);
                builderIndex += convert.Length;
                index += format.Length - 1;
                continue;
            }

            builderIndex++;
        }
    }
}
#endif