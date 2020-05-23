using System.IO;
using Verify;

class StreamConverter
{
    public string ToExtension { get; }
    public AsyncInstanceConversion<Stream> Func { get; }

    public StreamConverter(
        string toExtension,
        AsyncInstanceConversion<Stream> func)
    {
        ToExtension = toExtension;
        Func = func;
    }
}