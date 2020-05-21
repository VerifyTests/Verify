using System.Runtime.CompilerServices;
using Bunit;
using Xunit;
using Xunit.Abstractions;

namespace VerifyBunit
{
    public partial class VerifyBase :
        TestContext
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

        //TODO: move the ContextCleanup back into the dispose when bunit is released
        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //    XunitContext.Flush();
        //}

        //TODO: remove when bunit is released
        protected void Flush()
        {
            XunitContext.Flush();
        }
    }
}