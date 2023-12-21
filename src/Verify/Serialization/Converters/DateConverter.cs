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

        writer.WriteRawValueWithScrubbers(value.ToString("yyyy-MM-dd", Culture.InvariantCulture));
    }
}
#endif