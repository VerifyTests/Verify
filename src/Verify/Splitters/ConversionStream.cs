using System.IO;

namespace VerifyTests
{
    public readonly struct ConversionStream
    {
        readonly string? data;
        readonly Stream? stream;
        public string Extension { get; }

        public Stream Stream
        {
            get
            {
                if (stream == null)
                {
                    throw new("Use Data.");
                }
                return stream;
            }
        }
        public bool IsStream { get => stream != null; }

        public string Data
        {
            get
            {
                if (data == null)
                {
                    throw new("Use Stream.");
                }

                return data;
            }
        }
        public bool IsData { get => data != null; }

        public ConversionStream(string extension, Stream stream)
        {
            Guard.AgainstBadExtension(extension, nameof(extension));
            Guard.AgainstNull(stream, nameof(stream));

            if (EmptyFiles.Extensions.IsText(extension))
            {
                throw new("Dont pass a stream for text. Instead use the `ConversionStream(string extension, string data)`.");
            }

            Extension = extension;
            this.stream = stream;
            data = null;
        }

        public ConversionStream(string extension, string data)
        {
            Guard.AgainstBadExtension(extension, nameof(extension));
            Guard.AgainstNull(data, nameof(data));
            Extension = extension;
            this.data = data;
            stream = null;
        }
    }
}