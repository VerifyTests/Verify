using System;
using Verify;

class TypeConverter
{
    public string ToExtension { get; }
    public Func<object, VerifySettings, ConversionResult> Func { get; }
    public Func<Type, bool> CanConvert { get; }

    public TypeConverter(string toExtension, Func<object, VerifySettings, ConversionResult> func, Func<Type, bool> canConvert)
    {
        ToExtension = toExtension;
        Func = func;
        CanConvert = canConvert;
    }
}