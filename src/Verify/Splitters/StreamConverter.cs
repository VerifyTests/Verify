using System.IO;
using Verify;

class StreamConverter
{
    public string ToExtension { get; }
    public AsyncObjectConversion<Stream> Func { get; }

    public StreamConverter(
        string toExtension,
        AsyncObjectConversion<Stream> func)
    {
        ToExtension = toExtension;
        Func = func;
    }
}