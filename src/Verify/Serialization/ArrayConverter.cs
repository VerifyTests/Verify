class ArrayConverter : WriteOnlyJsonConverter
{
    public override bool CanConvert(Type objectType) =>
        true;

    public override void Write(VerifyJsonWriter writer, object value)
    {
        if (!writer.serialization.ShouldSerialize(value))
        {
            return;
        }

        var serializer = writer.Serializer;
        var currentTypeHandling = serializer.TypeNameHandling;
        if (currentTypeHandling == TypeNameHandling.Auto)
        {
            serializer.TypeNameHandling = TypeNameHandling.All;
        }

        writer.Serialize(value);

        serializer.TypeNameHandling = currentTypeHandling;
    }
}