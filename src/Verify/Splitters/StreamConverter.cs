using System.IO;
using VerifyTesting;

class StreamConverter
{
    public string ToExtension { get; }
    public AsyncConversion<Stream> Conversion { get; }

    public StreamConverter(
        string toExtension,
        AsyncConversion<Stream> conversion)
    {
        ToExtension = toExtension;
        Conversion = conversion;
    }
}