using System.Collections.Generic;

namespace VerifyTests
{
    public delegate ToAppend? JsonAppender(IReadOnlyDictionary<string, object> context);
}