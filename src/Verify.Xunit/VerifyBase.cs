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

        Verifier verifier;

        public VerifyBase(
            ITestOutputHelper output,
            [CallerFilePath] string sourceFile = "") :
            base(output, sourceFile)
        {
            var context = Context;
            verifier = new Verifier(context.TestType, context.SourceDirectory, context.UniqueTestName);
        }
    }
}