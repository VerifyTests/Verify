using System.IO;
using Verify;

class StreamConverter
{
    public string ToExtension { get; }
    public AsyncConversion<Stream> Func { get; }

    public StreamConverter(
        string toExtension,
        AsyncConversion<Stream> func)
    {
        ToExtension = toExtension;
        Func = func;
    }
}