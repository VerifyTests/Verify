readonly struct CultureDate(
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
    int dateSeparator,
    int timeSeparator,
    int eraLong = 0,
    int eraShort = 0)
{
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
    public int EraLong { get; } = eraLong;
    public int EraShort { get; } = eraShort;
}
