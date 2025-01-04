#if NET9_0
public class CultureToDateBuilder
{
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
            var info = culture.DateTimeFormat;

            var calendar = culture.Calendar;

                var am = info.AMDesignator;
                var pm = info. PMDesignator;
                string amPmLong;
                string amPmShort;
            if(am.Length < pm.Length)
            {
                amPmLong = pm;
                amPmShort = am;
            }
            else
            {
                amPmLong = am;
                amPmShort = pm;
            }

            var monthNames = Lengths(info.MonthNames);
            var abbreviatedMonthNames = Lengths(info.AbbreviatedMonthNames);
            var dayNames = Lengths(info.DayNames);
            var abbreviatedDayNames = Lengths(info.AbbreviatedDayNames);
            var eras = Lengths(calendar.Eras.Select(_ => info.GetEraName(_)));
            var timeSeparator = info.TimeSeparator;
            var dateSeparator = info.DateSeparator;
            builder.AppendLine(
                $$"""
                          {
                              "{{culture.Name}}",
                              new(
                                  // {{amPmLong}}
                                  {{amPmLong.Length}},
                                  // {{amPmShort}}
                                  {{amPmShort.Length}},
                                  // {{monthNames.LongValue}}
                                  {{monthNames.Long}},
                                  // {{monthNames.ShortValue}}
                                  {{monthNames.Short}},
                                  // {{abbreviatedMonthNames.LongValue}}
                                  {{abbreviatedMonthNames.Long}},
                                  // {{abbreviatedMonthNames.ShortValue}}
                                  {{abbreviatedMonthNames.Short}},
                                  // {{dayNames.LongValue}}
                                  {{dayNames.Long}},
                                  // {{dayNames.ShortValue}}
                                  {{dayNames.Short}},
                                  // {{abbreviatedDayNames.LongValue}}
                                  {{abbreviatedDayNames.Long}},
                                  // {{abbreviatedDayNames.ShortValue}}
                                  {{abbreviatedDayNames.Short}},
                                  // {{dateSeparator}}
                                  {{dateSeparator.Length}},
                                  // {{timeSeparator}}
                                  {{timeSeparator.Length}},
                                  // {{eras.LongValue}},
                                  {{eras.Long}},
                                  // {{eras.ShortValue}}
                                  {{eras.Short}})
                          },
                  """);
        }

        builder.AppendLine(
            """
                }.ToFrozenDictionary();
            }
            """);
        var file = Path.Combine(AttributeReader.GetSolutionDirectory(), "Verify/Serialization/Scrubbers/DateScrubber_Generated.cs");
        File.Delete(file);
        return File.WriteAllTextAsync(file, builder.ToString());
    }

    static (int Long, string LongValue, int Short, string ShortValue) Lengths(IEnumerable<string> names)
    {
        var lengths = names
            .Where(_ => _.Length > 0)
            .ToList();
        var max = lengths.Max()!;
        var min = lengths.Min()!;
        return (max.Length, max, min.Length, min);
    }
}
#endif