using Verify;

class TypeConverter
{
    public string? ToExtension { get; }
    public AsyncConversion Conversion { get; }
    public CanConvert CanConvert { get; }

    public TypeConverter(
        AsyncConversion conversion,
        CanConvert canConvert)
    {
        Conversion = conversion;
        CanConvert = canConvert;
    }

    public TypeConverter(
        string toExtension,
        AsyncConversion conversion,
        CanConvert canConvert)
    {
        ToExtension = toExtension;
        Conversion = conversion;
        CanConvert = canConvert;
    }
}