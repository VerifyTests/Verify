using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VerifyTests
{
    public class ConversionResult
    {
        public object? Info { get; }
        public IEnumerable<Stream> Streams { get; }
        public Func<Task>? Cleanup { get; }

        public ConversionResult(object? info, IEnumerable<Stream> streams, Func<Task>? cleanup = null)
        {
            Guard.AgainstNull(streams, nameof(streams));
            Info = info;
            Streams = streams;
            Cleanup = cleanup;
        }

        public ConversionResult(object? info, IEnumerable<Stream> streams)
        {
            Guard.AgainstNull(streams, nameof(streams));
            Info = info;
            Streams = streams;
        }

        public ConversionResult(object? info, Stream stream)
        {
            Guard.AgainstNull(stream, nameof(stream));
            Info = info;
            Streams = new List<Stream> {stream};
        }

        public ConversionResult(object? info, Stream stream, Func<Task>? cleanup = null)
        {
            Guard.AgainstNull(stream, nameof(stream));
            Info = info;
            Cleanup = cleanup;
            Streams = new List<Stream> {stream};
        }
    }
}