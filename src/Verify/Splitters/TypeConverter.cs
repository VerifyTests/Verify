class TypeConverter(
    AsyncConversion conversion,
    CanConvert canConvert)
{
    public AsyncConversion Conversion { get; } = conversion;
    public CanConvert CanConvert { get; } = canConvert;
}