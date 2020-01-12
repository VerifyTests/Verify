using System.Collections.Generic;
using System.IO;

namespace Verify
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
    }
}