using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Verify;

class StringScrubbingConverter :
    WriteOnlyJsonConverter
{
    internal static List<string> datetimeFormats = new List<string>();
    internal static List<string> datetimeOffsetFormats = new List<string>();

    Scrubber<Guid>? guidScrubber;
    Scrubber<DateTime>? dateTimeScrubber;
    Scrubber<DateTimeOffset>? dateTimeOffsetScrubber;

    public StringScrubbingConverter(Scrubber<Guid>? guidScrubber, Scrubber<DateTime>? dateTimeScrubber, Scrubber<DateTimeOffset>? dateTimeOffsetScrubber)
    {
        this.guidScrubber = guidScrubber;
        this.dateTimeScrubber = dateTimeScrubber;
        this.dateTimeOffsetScrubber = dateTimeOffsetScrubber;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            return;
        }

        var valueAsString = (string) value;
        if (!string.IsNullOrWhiteSpace(valueAsString))
        {
            if (guidScrubber != null)
            {
                if (Guid.TryParse(valueAsString, out var guid))
                {
                    guidScrubber.WriteValue(writer, guid);
                    return;
                }
            }

            if (dateTimeOffsetScrubber != null)
            {
                foreach (var format in datetimeOffsetFormats)
                {
                    if (DateTimeOffset.TryParseExact(valueAsString, format, null, DateTimeStyles.None, out var dateTimeOffset))
                    {
                        dateTimeOffsetScrubber.WriteValue(writer, dateTimeOffset);
                        return;
                    }
                }
            }

            if (dateTimeScrubber != null)
            {
                foreach (var format in datetimeFormats)
                {
                    if (DateTime.TryParseExact(valueAsString, format, null, DateTimeStyles.None, out var dateTime))
                    {
                        dateTimeScrubber.WriteValue(writer, dateTime);
                        return;
                    }
                }
            }
        }

        writer.WriteValue(valueAsString);
    }

    public override bool CanRead => false;

    public override bool CanConvert(Type type)
    {
        return type == typeof(string);
    }
}