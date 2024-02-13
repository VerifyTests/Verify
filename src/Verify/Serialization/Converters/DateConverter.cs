#if NET6_0_OR_GREATER
class DateConverter :
    WriteOnlyJsonConverter<Date>
{
    public override void Write(VerifyJsonWriter writer, Date value)
    {
        if (writer.serialization.TryConvert(writer.Counter, value, out var result))
        {
            writer.WriteRawValueIfNoStrict(result);
            return;
        }

        Span<char> buffer = stackalloc char[10];
        value.TryFormat(buffer, out _, "yyyy-MM-dd".AsSpan(), Culture.InvariantCulture);
        writer.WriteRawValueWithScrubbers(buffer);
    }
}
#endif