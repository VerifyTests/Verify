using System.Collections.Generic;
using System.IO;

namespace Verify
{
    public class ConversionResult
    {
        public object? MetaData { get; }
        public IEnumerable<Stream> Streams { get; }

        public ConversionResult(object? metaData, IEnumerable<Stream> streams)
        {
            Guard.AgainstNull(streams, nameof(streams));
            MetaData = metaData;
            Streams = streams;
        }
    }
}