using System.Linq;
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
            var name = TestNameBuilder.GetUniqueTestName(context.TestType, context.MethodInfo, context.Parameters.Select(x=>x.Value).ToList());
            return new InnerVerifier(context.TestType, context.SourceDirectory, name);
        }
    }
}