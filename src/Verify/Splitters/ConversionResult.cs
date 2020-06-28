using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VerifyTests
{
    public class ConversionResult
    {
        public object? Info { get; }
        public IEnumerable<ConversionStream> Streams { get; }
        public Func<Task>? Cleanup { get; }

        public ConversionResult(object? info, IEnumerable<ConversionStream> streams, Func<Task>? cleanup = null)
        {
            Guard.AgainstNull(streams, nameof(streams));
            Info = info;
            Streams = streams;
            Cleanup = cleanup;
        }

        public ConversionResult(object? info, string streamExtension, Stream stream, Func<Task>? cleanup = null)
        {
            Guard.AgainstNull(stream, nameof(stream));
            Guard.AgainstNullOrEmpty(streamExtension, nameof(streamExtension));
            Info = info;
            Cleanup = cleanup;
            Streams = new List<ConversionStream>
            {
                new ConversionStream(streamExtension, stream)
            };
        }
    }
    public class ConversionStream
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