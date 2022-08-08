class EncodingConverter :
    WriteOnlyJsonConverter<Encoding>
{
    public override void Write(VerifyJsonWriter writer, Encoding value) =>
        writer.WriteSingleLineNoScrubbing(value.EncodingName);
}