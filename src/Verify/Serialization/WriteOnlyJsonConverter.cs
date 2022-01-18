using Newtonsoft.Json;

namespace VerifyTests;

public abstract class WriteOnlyJsonConverter :
    JsonConverter
{
    public sealed override bool CanRead => false;

    public sealed override void WriteJson(
        JsonWriter writer,
        object? value,
        JsonSerializer serializer)
    {
        if (value is null)
        {
            return;
        }

        WriteJson((VerifyJsonWriter)writer, value, serializer);
    }

    public abstract void WriteJson(
        VerifyJsonWriter writer,
        object value,
        JsonSerializer serializer);

    public sealed override object ReadJson(
        JsonReader reader,
        Type type,
        object? value,
        JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}