using System.Collections.Generic;

namespace VerifyTests
{
    public delegate Target? FileAppender(IReadOnlyDictionary<string, object> context);
}