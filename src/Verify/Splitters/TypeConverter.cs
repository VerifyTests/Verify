using System;
using Verify;

class TypeConverter
{
    public string? ToExtension { get; }
    public AsyncObjectConversion<object> Func { get; }
    public Func<Type, bool> CanConvert { get; }

    public TypeConverter(
        AsyncObjectConversion<object> func,
        Func<Type, bool> canConvert)
    {
        Func = func;
        CanConvert = canConvert;
    }

    public TypeConverter(
        string toExtension,
        AsyncObjectConversion<object> func,
        Func<Type, bool> canConvert)
    {
        ToExtension = toExtension;
        Func = func;
        CanConvert = canConvert;
    }
}