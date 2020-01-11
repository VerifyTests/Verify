using System;
using System.Collections.Generic;
using System.IO;
using Verify;

class TypeConverter
{
    public string ToExtension { get; }
    public Func<object, VerifySettings, IEnumerable<Stream>> Func { get; }
    public Func<Type, bool> CanConvert { get; }

    public TypeConverter(string toExtension, Func<object, VerifySettings, IEnumerable<Stream>> func, Func<Type, bool> canConvert)
    {
        ToExtension = toExtension;
        Func = func;
        CanConvert = canConvert;
    }
}