using System.Runtime.CompilerServices;
using Xunit;
using Xunit.Abstractions;

namespace VerifyXunit
{
    public partial class VerifyBase :
        XunitContextBase
    {
        public VerifyBase(
            ITestOutputHelper output,
            [CallerFilePath] string sourceFile = "") :
            base(output, sourceFile)
        {
        }

        InnerVerifier GetVerifier()
        {
            var context = Context;
            return new InnerVerifier(context.TestType, context.SourceDirectory, context.UniqueTestName);
        }
    }
}