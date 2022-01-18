using System.Globalization;
using Newtonsoft.Json;
using VerifyTests;

class GuidConverter :
    WriteOnlyJsonConverter<Guid>
{
    SharedScrubber scrubber;

    public GuidConverter(SharedScrubber scrubber)
    {
        this.scrubber = scrubber;
    }

    public override void WriteJson(
        VerifyJsonTextWriter writer,
        Guid value,
        JsonSerializer serializer)
    {
        if (scrubber.TryConvert(value, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(value.ToString("D", CultureInfo.InvariantCulture));
    }
}