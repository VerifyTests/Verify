class IdConverter :
    WriteOnlyJsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override void Write(VerifyJsonWriter writer, object value)
    {
        var id = writer.Counter.NextId(value);
        writer.WriteValue($"Id_{id}");
    }
}