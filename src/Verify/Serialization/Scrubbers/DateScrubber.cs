#if NET6_0_OR_GREATER
static class DateScrubber
{
    public static void ReplaceDates(StringBuilder builder, Counter counter)
    {
        string format = "yyyy-MM-dd";
        var value = builder.ToString().AsSpan();

        builder.Clear();
        for (var index = 0; index <= value.Length; index++)
        {
            var end = index + format.Length;
            if (end > value.Length)
            {
                var remaining = value[index..];
                builder.Append(remaining);
                return;
            }

            if (index == 0 &&
                end == value.Length)
            {
                var substring = value.Slice(index, format.Length);
                if (Date.TryParseExact(substring, format, out var date))
                {
                    var convert = SerializationSettings.Convert(counter, date);
                    builder.Append(convert);
                    index += format.Length-1;
                    continue;
                }
            }

            builder.Append(value[index]);
        }
    }
}
#endif