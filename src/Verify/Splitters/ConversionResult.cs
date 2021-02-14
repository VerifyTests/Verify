using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VerifyTests
{
    public readonly struct ConversionResult
    {
        public object? Info { get; }

        public IEnumerable<Target> Targets { get; }

        public Func<Task>? Cleanup { get; }

        public ConversionResult(object? info, IEnumerable<Target> streams, Func<Task>? cleanup = null)
        {
            Guard.AgainstNull(streams, nameof(streams));
            Info = info;
            Targets = streams;
            Cleanup = cleanup;
        }

        public ConversionResult(object? info, string streamExtension, Stream stream, Func<Task>? cleanup = null)
        {
            Guard.AgainstNull(stream, nameof(stream));
            Guard.AgainstNullOrEmpty(streamExtension, nameof(streamExtension));
            Info = info;
            Cleanup = cleanup;
            Targets = new List<Target>
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
            Targets = new List<Target>
            {
                new(streamExtension, data)
            };
        }
    }
}