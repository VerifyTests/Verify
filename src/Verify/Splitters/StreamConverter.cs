using System.IO;
using VerifyTests;

class StreamConverter
{
    public AsyncConversion<Stream> Conversion { get; }

    public StreamConverter(
        AsyncConversion<Stream> conversion)
    {
        Conversion = conversion;
    }
}