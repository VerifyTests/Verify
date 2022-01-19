using System.Globalization;
using Newtonsoft.Json;
using VerifyTests;

class GuidConverter :
    WriteOnlyJsonConverter<Guid>
{
    SerializationSettings scrubber;

    public GuidConverter(SerializationSettings scrubber)
    {
        this.scrubber = scrubber;
    }

    public override void Write(
        VerifyJsonWriter writer,
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