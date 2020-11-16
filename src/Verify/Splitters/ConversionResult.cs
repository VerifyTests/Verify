using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VerifyTests
{
    public readonly struct ConversionResult
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
                new(streamExtension, stream)
            };
        }

        public ConversionResult(object? info, string streamExtension, string data, Func<Task>? cleanup = null)
        {
            Guard.AgainstNull(data, nameof(data));
            Guard.AgainstNullOrEmpty(streamExtension, nameof(streamExtension));
            Info = info;
            Cleanup = cleanup;
            Streams = new List<ConversionStream>
            {
                new(streamExtension, data)
            };
        }
    }
}