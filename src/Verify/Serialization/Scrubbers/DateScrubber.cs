#if NET6_0_OR_GREATER
class CultureDates
{
    public DateTimeOffset Long { get; set; }
    public DateTimeOffset Short { get; set; }
}
static class DateScrubber
{
    static Date longDate = new(2023, 12, 20);

    public static DayOfWeek LongestDayName(this DateTimeFormatInfo formatInfo)
    {
        var names = formatInfo.DayNames;
        var longestIndex = names[0].Length;
        var longest = names[0];
        for (var index = 1; index < names.Length; index++)
        {
            var name = names[index];
            if (name.Length > longest.Length)
            {
                longest = name;
                longestIndex = index;
            }
        }

        return (DayOfWeek) longestIndex;
    }
    public static int LongestMonthName(this DateTimeFormatInfo formatInfo)
    {
        var names = formatInfo.MonthNames;
        var longestIndex = names[0].Length;
        var longest = names[0];
        for (var index = 1; index < names.Length; index++)
        {
            var name = names[index];
            if (name.Length > longest.Length)
            {
                longest = name;
                longestIndex = index;
            }
        }

        return longestIndex+1;
    }

    public static IEnumerable<int> Lengths(string format, CultureInfo culture)
    {
        var dateTimeFormat = culture.DateTimeFormat;
        var longDateName = dateTimeFormat.DayNames.OrderBy(_=>_.Length).First();
        var longMonthName = dateTimeFormat.MonthNames.OrderBy(_=>_.Length).First();
        // var longDateName = dateTimeFormat.DayNames.OrderBy(_=>_.Length).First();
        // var longMonthName = dateTimeFormat.MonthNames.OrderBy(_=>_.Length).First();
        // int dayIndex = 0;
        // int dayLength = 0;
        // foreach (var VARIABLE in culture.DateTimeFormat.DayNames)
        // {
        //
        // }
        var longLength = longDate.ToString(format, culture)
            .Length;
        var shortLength = Date.MinValue.ToString(format, culture)
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