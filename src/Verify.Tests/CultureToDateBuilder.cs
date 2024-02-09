#if NET8_0
public class CultureToDateBuilder
{
    const string monthDayFormat = "MMMM MMM dddd ddd";

    [Fact]
    public Task BuildCultureToDate()
    {
        if (Environment.OSVersion.Version.Build < 22000)
        {
            return Task.CompletedTask;
        }
        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
        var builder = new StringBuilder(
            """
            // <auto-generated />
            static partial class DateScrubber
            {
                static IReadOnlyDictionary<string, CultureDate> cultureDates = new Dictionary<string, CultureDate>()
                {

            """);
        foreach (var culture in cultures)
        {
            var (longDate, shortDate) = FindDates(culture);
            if (culture.Name.Contains('-'))
            {
                var (parentLongDate, parentShortDate) = FindDates(culture.Parent);
                if (longDate == parentLongDate && shortDate == parentShortDate)
                {
                    continue;
                }
            }
            builder.AppendLine(
                $$"""
                          {
                              "{{culture.Name}}",
                              new(
                                  new(2023, {{longDate.Month}}, {{longDate.Day}}, {{longDate.Hour}}, 10, 10, 10),
                                  new(2023, {{shortDate.Month}}, {{shortDate.Day}}, {{shortDate.Hour}}, 0, 0))
                          },
                  """);
        }

        builder.AppendLine(
            """
                }
            #if NET8_0_OR_GREATER
                        .ToFrozenDictionary()
            #endif
                 ;
            }
            """);
        var file = Path.Combine(AttributeReader.GetSolutionDirectory(), "Verify/Serialization/Scrubbers/DateScrubber_Generated.cs");
        File.Delete(file);
        return File.WriteAllTextAsync(file, builder.ToString());
    }

    static (DateTime longDate, DateTime shortDate) FindDates(CultureInfo culture)
    {
        var formatInfo = culture.DateTimeFormat;
        var longDate = FindLongDate(formatInfo);
        var shortDate = FindShortDate(formatInfo);
        return (longDate, shortDate);
    }

    static DateTime FindLongDate(DateTimeFormatInfo formatInfo)
    {
        DateTime longDate = default;
        var longFormatted = "";
        var amLength = formatInfo.AMDesignator.Length;
        var pmLength = formatInfo.PMDesignator.Length;
        for (var month = 1; month <= 12; month++)
        {
            for (var day = 20; day <= 27; day++)
            {
                DateTime date;
                if (amLength > pmLength)
                {
                    date = new(2023, month, day, 1, 0, 0, 0);
                }
                else
                {
                    date = new(2023, month, day, 12, 0, 0, 0);
                }

                var formatted = date.ToString(monthDayFormat, formatInfo);
                if (formatted.Length > longFormatted.Length)
                {
                    longFormatted = formatted;
                    longDate = date;
                }
            }
        }

        return longDate;
    }

    static DateTime FindShortDate(DateTimeFormatInfo formatInfo)
    {
        DateTime shortDate = default;
        string? shortFormatted = null;
        var amLength = formatInfo.AMDesignator.Length;
        var pmLength = formatInfo.PMDesignator.Length;
        for (var month = 1; month <= 12; month++)
        {
            for (var day = 1; day <= 7; day++)
            {
                DateTime date;
                if (amLength <= pmLength)
                {
                    date = new(2023, month, day, 1, 0, 0, 0);
                }
                else
                {
                    date = new(2023, month, day, 13, 0, 0, 0);
                }

                var formatted = date.ToString(monthDayFormat, formatInfo);
                if (shortFormatted == null ||
                    formatted.Length < shortFormatted.Length)
                {
                    shortFormatted = formatted;
                    shortDate = date;
                }
            }
        }

        return shortDate;
    }
}
#endif