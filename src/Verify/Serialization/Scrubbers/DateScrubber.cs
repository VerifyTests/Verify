#if NET6_0_OR_GREATER
static class DateScrubber
{
    public static void ReplaceDates(StringBuilder builder, Counter counter)
    {
        string format = "yyyy-MM-dd";
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