using System;
using System.Collections.Generic;
using System.IO;

class StreamConverter
{
    public string ToExtension { get; }
    public Func<Stream, IEnumerable<Stream>> Func { get; }

    public StreamConverter(string toExtension, Func<Stream, IEnumerable<Stream>> func)
    {
        ToExtension = toExtension;
        Func = func;
    }
}