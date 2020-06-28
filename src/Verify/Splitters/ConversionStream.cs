using System.IO;

namespace VerifyTests
{
    public readonly struct ConversionStream
    {
        public string Extension { get; }
        public Stream Stream { get; }

        public ConversionStream(string extension, Stream stream)
        {
            Guard.AgainstBadExtension(extension, nameof(extension));
            Guard.AgainstNull(stream, nameof(stream));
            Extension = extension;
            Stream = stream;
        }
    }
}