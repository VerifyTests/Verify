using System.Runtime.CompilerServices;
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
            Verifier.Init(
                message => new XunitException(message),
                input => XunitContext.Context.IntOrNext(input),
                input => XunitContext.Context.IntOrNext(input),
                input => XunitContext.Context.IntOrNext(input),
                Assert.Equal);
        }

        public VerifyBase(
            ITestOutputHelper output,
            [CallerFilePath] string sourceFile = "") :
            base(output, sourceFile)
        {
        }

        Verifier GetVerifier()
        {
            var context = Context;
            return new Verifier(context.TestType, context.SourceDirectory, context.UniqueTestName);
        }
    }
}