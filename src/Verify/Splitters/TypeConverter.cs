using Verify;

class TypeConverter
{
    public string? ToExtension { get; }
    public AsyncInstanceConversion Func { get; }
    public CanConvert CanConvert { get; }

    public TypeConverter(
        AsyncInstanceConversion func,
        CanConvert canConvert)
    {
        Func = func;
        CanConvert = canConvert;
    }

    public TypeConverter(
        string toExtension,
        AsyncInstanceConversion func,
        CanConvert canConvert)
    {
        ToExtension = toExtension;
        Func = func;
        CanConvert = canConvert;
    }
}