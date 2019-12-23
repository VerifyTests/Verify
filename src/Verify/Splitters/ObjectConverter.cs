using System;
using System.Collections.Generic;
using System.IO;

class ObjectConverter
{
    public string ToExtension { get; }
    public Func<object, IEnumerable<Stream>> Func { get; }

    public ObjectConverter(string toExtension, Func<object, IEnumerable<Stream>> func)
    {
        ToExtension = toExtension;
        Func = func;
    }
}