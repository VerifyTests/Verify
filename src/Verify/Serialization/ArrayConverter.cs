class ArrayConverter : WriteOnlyJsonConverter
{
    public override bool CanConvert(Type objectType) =>
        true;

    public override void Write(VerifyJsonWriter writer, object value)
    {
        if (!writer.settings.ShouldSerialize(value))
        {
            return;
        }

        writer.Serialize(value);
    }
}