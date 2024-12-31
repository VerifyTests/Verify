readonly struct CultureDate(DateTime longDate, DateTime shortDate, int amPmLong = 0, int amPmShort = 0)
{
    public DateTime Long { get; } = longDate;
    public DateTime Short { get; } = shortDate;
    public int AmPmLong { get; } = amPmLong;
    public int AmPmShort { get; } = amPmShort;
}
