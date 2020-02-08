using System;
using System.Threading.Tasks;
using Verify;

class TypeConverter
{
    public string? ToExtension { get; }
    public Func<object, VerifySettings, Task<ConversionResult>> Func { get; }
    public Func<Type, bool> CanConvert { get; }

    public TypeConverter(
        Func<object, VerifySettings, Task<ConversionResult>> func,
        Func<Type, bool> canConvert)
    {
        Func = func;
        CanConvert = canConvert;
    }

    public TypeConverter(
        string toExtension,
        Func<object, VerifySettings, Task<ConversionResult>> func,
        Func<Type, bool> canConvert)
    {
        ToExtension = toExtension;
        Func = func;
        CanConvert = canConvert;
    }
}