using System;
using System.IO;
using Verify;

class StreamConverter
{
    public string ToExtension { get; }
    public Func<Stream, VerifySettings, ConversionResult> Func { get; }

    public StreamConverter(
        string toExtension,
        Func<Stream, VerifySettings, ConversionResult> func)
    {
        ToExtension = toExtension;
        Func = func;
    }
}