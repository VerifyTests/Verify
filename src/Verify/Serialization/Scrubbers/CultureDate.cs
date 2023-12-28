readonly struct CultureDate(DateTime longDate, DateTime shortDate)
{
    public DateTime Long { get; } = longDate;
    public DateTime Short { get; } = shortDate;
}