using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VerifyTests
{
    public class ConversionResult
    {
        public object? Info { get; }
        public string StreamExtension { get; }
        public IEnumerable<Stream> Streams { get; }
        public Func<Task>? Cleanup { get; }

        public ConversionResult(object? info, string streamExtension, IEnumerable<Stream> streams, Func<Task>? cleanup = null)
        {
            Guard.AgainstNull(streams, nameof(streams));
            Guard.AgainstNullOrEmpty(streamExtension, nameof(streamExtension));
            Info = info;
            StreamExtension = streamExtension;
            Streams = streams;
            Cleanup = cleanup;
        }

        public ConversionResult(object? info, string streamExtension, Stream stream, Func<Task>? cleanup = null)
        {
            Guard.AgainstNull(stream, nameof(stream));
            Guard.AgainstNullOrEmpty(streamExtension, nameof(streamExtension));
            Info = info;
            StreamExtension = streamExtension;
            Cleanup = cleanup;
            Streams = new List<Stream> {stream};
        }
    }
}