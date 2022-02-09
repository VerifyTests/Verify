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

        Write((VerifyJsonWriter)writer, value);
    }

    public abstract void Write(VerifyJsonWriter writer, object value);

    public sealed override object ReadJson(
        JsonReader reader,
        Type type,
        object? value,
        JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}