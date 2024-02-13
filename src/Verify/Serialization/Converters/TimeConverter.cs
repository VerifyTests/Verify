#if NET6_0_OR_GREATER
class TimeConverter :
    WriteOnlyJsonConverter<Time>
{
    public override void Write(VerifyJsonWriter writer, Time value)
    {
        if (writer.serialization.TryConvert(writer.Counter, value, out var result))
        {
            writer.WriteRawValueIfNoStrict(result);
            return;
        }

        Span<char> buffer = stackalloc char[7];
        value.TryFormat(buffer, out _, "h:mm tt".AsSpan(), Culture.InvariantCulture);
        writer.WriteRawValueWithScrubbers(buffer);
    }
}
#endif