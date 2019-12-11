using System;
using System.Runtime.CompilerServices;
using Verify;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace VerifyXunit
{
    public partial class VerifyBase :
        XunitContextBase
    {
        static VerifyBase()
        {
            Scrubber<Guid>.SetIntOrNext(input => XunitContext.Context.IntOrNext(input));
            Scrubber<DateTime>.SetIntOrNext(input => XunitContext.Context.IntOrNext(input));
            Scrubber<DateTimeOffset>.SetIntOrNext(input => XunitContext.Context.IntOrNext(input));
        }

        static Func<string, Exception> exceptionBuilder = s => new XunitException(s);

        public VerifyBase(
            ITestOutputHelper output,
            [CallerFilePath] string sourceFile = "") :
            base(output, sourceFile)
        {
        }
    }
}