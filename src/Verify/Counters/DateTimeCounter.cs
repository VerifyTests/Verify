using System;

class DateTimeCounter :
    Counter<DateTime>
{
    static DateTime Start = new(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc);

    protected override DateTime Convert(int i)
    {
        return Start.AddDays(i);
    }
}