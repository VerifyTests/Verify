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

        writer.Serialize(value);
    }
}