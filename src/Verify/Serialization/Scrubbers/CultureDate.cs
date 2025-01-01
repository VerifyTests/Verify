readonly struct CultureDate(
    DateTime longDate,
    DateTime shortDate,
    int amPmLong,
    int amPmShort,
    int monthNameLong,
    int monthNameShort,
    int abbreviatedMonthNameLong,
    int abbreviatedMonthNameShort,
    int dayNameLong,
    int dayNameShort,
    int abbreviatedDayNameLong,
    int abbreviatedDayNameShort,
    int dateSeparator = 0,
    int timeSeparator = 0)
{
    public DateTime Long { get; } = longDate;
    public DateTime Short { get; } = shortDate;
    public int AmPmLong { get; } = amPmLong;
    public int AmPmShort { get; } = amPmShort;
    public int MonthNameLong { get; } = monthNameLong;
    public int MonthNameShort { get; } = monthNameShort;
    public int AbbreviatedMonthNameLong { get; } = abbreviatedMonthNameLong;
    public int AbbreviatedMonthNameShort { get; } = abbreviatedMonthNameShort;
    public int DayNameLong { get; } = dayNameLong;
    public int DayNameShort { get; } = dayNameShort;
    public int AbbreviatedDayNameLong { get; } = abbreviatedDayNameLong;
    public int AbbreviatedDayNameShort { get; } = abbreviatedDayNameShort;
    public int DateSeparator { get; } = dateSeparator;
    public int TimeSeparator { get; } = timeSeparator;
}
