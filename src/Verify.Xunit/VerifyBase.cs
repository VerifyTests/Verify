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
            //TODO merge contexts
            Verifier.Init(
                message => new XunitException(message),
                input => XunitContext.Context.IntOrNext(input),
                input => XunitContext.Context.IntOrNext(input),
                input => XunitContext.Context.IntOrNext(input),
                Assert.Equal, () =>
                {
                    var context = XunitContext.Context;
                    return new TestContext(context.TestType, context.SourceDirectory, context.UniqueTestName);
                });
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