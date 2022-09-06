#if NET6_0_OR_GREATER
class TimeConverter :
    WriteOnlyJsonConverter<TimeOnly>
{
    public override void Write(VerifyJsonWriter writer, TimeOnly value)
    {
        if (writer.serialization.TryConvert(writer.Counter, value, out var result))
        {
            writer.WriteRawValueIfNoStrict(result);
            return;
        }

        writer.WriteRawValueWithScrubbers(value.ToString("h:mm tt", CultureInfo.InvariantCulture));
    }
}
#endif