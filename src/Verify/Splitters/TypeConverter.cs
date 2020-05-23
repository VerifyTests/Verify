using Verify;

class TypeConverter
{
    public string? ToExtension { get; }
    public AsyncConversion Func { get; }
    public CanConvert CanConvert { get; }

    public TypeConverter(
        AsyncConversion func,
        CanConvert canConvert)
    {
        Func = func;
        CanConvert = canConvert;
    }

    public TypeConverter(
        string toExtension,
        AsyncConversion func,
        CanConvert canConvert)
    {
        ToExtension = toExtension;
        Func = func;
        CanConvert = canConvert;
    }
}