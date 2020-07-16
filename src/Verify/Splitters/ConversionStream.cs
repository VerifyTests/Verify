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

        public ConversionStream(string extension, string data)
        {
            Guard.AgainstBadExtension(extension, nameof(extension));
            Guard.AgainstNull(data, nameof(data));
            Extension = extension;
            Stream = new MemoryStream(FileHelpers.Utf8NoBOM.GetBytes(data));
        }
    }
}