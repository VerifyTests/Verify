static class DateFormatExpander
{
    internal static string ExpandFormat(this DateTimeFormatInfo info, string format)
    {
        if (format.Length != 1)
        {
            return format;
        }

        var ch = format[0];
        return ch switch
        {
            'd' => info.ShortDatePattern,
            'D' => info.LongDatePattern,
            'f' => $"{info.LongDatePattern} {info.ShortTimePattern}",
            'F' => info.FullDateTimePattern,
            'g' => $"{info.ShortDatePattern} {info.ShortTimePattern}",
            'G' => $"{info.ShortDatePattern} {info.LongTimePattern}",
            'm' or 'M' => info.MonthDayPattern,
            'o' or 'O' => "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK",
            'r' or 'R' => info.RFC1123Pattern,
            's' => info.SortableDateTimePattern,
            't' => info.ShortTimePattern,
            'T' => info.LongTimePattern,
            'u' => info.UniversalSortableDateTimePattern,
            'U' => info.FullDateTimePattern,
            'y' or 'Y' => info.YearMonthPattern,
            _ => throw new($"Invalid format: {format}"),
        };
    }
}