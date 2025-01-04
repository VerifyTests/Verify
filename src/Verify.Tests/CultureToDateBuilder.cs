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
            var formatInfo = culture.DateTimeFormat;

            var calendar = culture.Calendar;

            var amLength = formatInfo.AMDesignator.Length;
            var pmLength = formatInfo.PMDesignator.Length;
            var dateSeparator = formatInfo.DateSeparator.Length;
            var timeSeparator = formatInfo.TimeSeparator.Length;
            var monthNames = Lengths(formatInfo.MonthNames);
            var abbreviatedMonthNames = Lengths(formatInfo.AbbreviatedMonthNames);
            var dayNames = Lengths(formatInfo.DayNames);
            var abbreviatedDayNames = Lengths(formatInfo.AbbreviatedDayNames);
            var eras = Lengths(calendar.Eras.Select(_ => formatInfo.GetEraName(_)));
            builder.AppendLine(
                $$"""
                          {
                              "{{culture.Name}}",
                              new(
                                  {{int.Max(amLength, pmLength)}},
                                  {{int.Min(amLength, pmLength)}},
                                  {{monthNames.Long}},
                                  {{monthNames.Short}},
                                  {{abbreviatedMonthNames.Long}},
                                  {{abbreviatedMonthNames.Short}},
                                  {{dayNames.Long}},
                                  {{dayNames.Short}},
                                  {{abbreviatedDayNames.Long}},
                                  {{abbreviatedDayNames.Short}},
                                  {{dateSeparator}},
                                  {{timeSeparator}},
                                  {{eras.Long}},
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

    static (int Long, int Short) Lengths(IEnumerable<string> names)
    {
        var lengths = names
            .Select(_ => _.Length)
            .Where(_ => _ > 0)
            .ToList();
        return (lengths.Max(), lengths.Min());
    }
}
#endif