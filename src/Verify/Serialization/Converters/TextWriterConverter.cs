using Newtonsoft.Json;
using VerifyTests;

class TextWriterConverter :
    WriteOnlyJsonConverter<TextWriter>
{
    SerializationSettings settings;

    public TextWriterConverter(SerializationSettings settings)
    {
        this.settings = settings;
    }

    public override void Write(
        VerifyJsonWriter writer,
        TextWriter value,
        JsonSerializer serializer)
    {
        var stringValue = value.ToString();
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (stringValue is null)
        {
            return;
        }

        if (settings.TryConvertString(stringValue, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(stringValue);
    }
}