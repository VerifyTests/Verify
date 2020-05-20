using System.Runtime.CompilerServices;
using Bunit;
using Xunit;
using Xunit.Abstractions;

namespace VerifyBunit
{
    public partial class VerifyBase :
        ComponentTestFixture
    {
        public VerifyBase(
            ITestOutputHelper output,
            [CallerFilePath] string sourceFile = "")
        {
            XunitContext.Register(output, sourceFile);
        }

        InnerVerifier GetVerifier()
        {
            var context = XunitContext.Context;
            return new InnerVerifier(context.TestType, context.SourceDirectory, context.UniqueTestName);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            XunitContext.Flush();
        }
    }
}