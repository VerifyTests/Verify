using Newtonsoft.Json;
using VerifyTests;

class IdConverter :
    WriteOnlyJsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override void Write(
        VerifyJsonWriter writer,
        object value,
        JsonSerializer serializer)
    {
        var id = CounterContext.Current.NextId(value);
        writer.WriteValue($"Id_{id}");
    }
}