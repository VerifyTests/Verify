using System;
using System.Collections.Generic;
using System.IO;
using Verify;

class StreamConverter
{
    public string ToExtension { get; }
    public Func<Stream, VerifySettings, IEnumerable<Stream>> Func { get; }

    public StreamConverter(string toExtension, Func<Stream, VerifySettings, IEnumerable<Stream>> func)
    {
        ToExtension = toExtension;
        Func = func;
    }
}