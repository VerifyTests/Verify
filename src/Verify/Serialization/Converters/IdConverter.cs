class IdConverter :
    WriteOnlyJsonConverter
{
    public override bool CanConvert(Type objectType) =>
        true;

    public override void Write(VerifyJsonWriter writer, object value)
    {
        var id = writer.Counter.NextIdString(value);
        writer.WriteValue(id);
    }
}