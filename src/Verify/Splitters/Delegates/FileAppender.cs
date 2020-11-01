using System.Collections.Generic;

namespace VerifyTests
{
    public delegate ConversionStream? FileAppender(IReadOnlyDictionary<string, object> context);
}