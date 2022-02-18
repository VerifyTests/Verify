class TypeConverter
{
    public AsyncConversion Conversion { get; }
    public CanConvert CanConvert { get; }

    public TypeConverter(
        AsyncConversion conversion,
        CanConvert canConvert)
    {
        Conversion = conversion;
        CanConvert = canConvert;
    }
}