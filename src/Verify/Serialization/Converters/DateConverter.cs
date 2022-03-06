#if NET6_0_OR_GREATER
class DateConverter :
    WriteOnlyJsonConverter<DateOnly>
{
    public override void Write(VerifyJsonWriter writer, DateOnly value)
    {
        if (writer.settings.TryConvert(writer.Counter, value, out var result))
        {
            writer.WriteRawValue(result);
            return;
        }

        writer.WriteValue(value);
    }
}
#endif