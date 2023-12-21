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

        writer.WriteRawValueWithScrubbers(value.ToString("h:mm tt", Culture.InvariantCulture));
    }
}
#endif