using System;
using System.IO;
using System.Threading.Tasks;
using Verify;

class StreamConverter
{
    public string ToExtension { get; }
    public Func<Stream, VerifySettings, Task<ConversionResult>> Func { get; }

    public StreamConverter(
        string toExtension,
        Func<Stream, VerifySettings, Task<ConversionResult>> func)
    {
        ToExtension = toExtension;
        Func = func;
    }
}