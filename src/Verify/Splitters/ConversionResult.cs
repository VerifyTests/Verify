using System.Collections.Generic;
using System.IO;

namespace VerifyTests
{
    public class ConversionResult
    {
        public object? Info { get; }
        public IEnumerable<Stream> Streams { get; }

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
    }
}