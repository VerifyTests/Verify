readonly struct CultureDate(
    DateTime longDate,
    DateTime shortDate,
    int amPmLong,
    int amPmShort,
    int monthNameLong,
    int monthNameShort,
    int abbreviatedMonthNameLong = 0,
    int abbreviatedMonthNameShort = 0)
{
    public DateTime Long { get; } = longDate;
    public DateTime Short { get; } = shortDate;
    public int AmPmLong { get; } = amPmLong;
    public int AmPmShort { get; } = amPmShort;
    public int MonthNameLong { get; } = monthNameLong;
    public int MonthNameShort { get; } = monthNameShort;
    public int AbbreviatedMonthNameLong { get; } = abbreviatedMonthNameLong;
    public int AbbreviatedMonthNameShort { get; } = abbreviatedMonthNameShort;
}
